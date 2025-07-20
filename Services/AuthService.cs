using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using AskHire_Backend.Interfaces.Services;

namespace AskHire_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository userRepository, IConfiguration configuration,
                           UserManager<User> userManager, SignInManager<User> signInManager,
                           IAuthEmailService emailService,
                           ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogWarning("Login failed for email {Email}: Invalid credentials.", request.Email);
                return null;
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning("Login attempt failed for {Email}: Email not confirmed.", request.Email);
                throw new ApplicationException("Your email address has not been confirmed. Please check your inbox for a verification link.");
            }

            _logger.LogInformation("User {Email} successfully authenticated.", request.Email);
            return await CreateTokenResponse(user);
        }

        public async Task<User?> RegisterAsync(UserRegisterDto request)
        {
            if (await _userRepository.AnyAsync(request.Email))
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists.", request.Email);
                return null;
            }

            if (!IsAdult(request.DOB))
            {
                return null;
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                DOB = request.DOB,
                NIC = request.NIC,
                MobileNumber = request.MobileNumber,
                Address = request.Address,
                Role = "Candidate",
                SignUpDate = DateTime.UtcNow,
                ProfilePictureUrl = "/avatars/avatar2.png",
                EmailConfirmed = false
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    _logger.LogError("Identity error during user creation for {Email}: {Code} - {Description}", request.Email, error.Code, error.Description);
                }
                return null;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, user.Role);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    _logger.LogError("Identity error assigning role to {Email}: {Code} - {Description}", user.Email, error.Code, error.Description);
                }
                await _userManager.DeleteAsync(user);
                return null;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var backendBaseUrl = _configuration["BackendBaseUrl"];
            if (string.IsNullOrEmpty(backendBaseUrl))
            {
                _logger.LogError("BackendBaseUrl is not configured in appsettings.json. Cannot generate email confirmation link.");
                throw new InvalidOperationException("BackendBaseUrl is not configured. Email verification link cannot be generated.");
            }

            var callbackUrl = QueryHelpers.AddQueryString($"{backendBaseUrl}/api/Auth/confirm-email", new Dictionary<string, string?>
            {
                {"userId", user.Id.ToString()},
                {"token", encodedToken}
            });

            var emailSubject = "Confirm your email for AskHire Account";
            var emailMessage = $"Dear {user.FirstName},<br><br>" +
                               "Thank you for registering with AskHire. Please confirm your account by clicking the link below:<br><br>" +
                               $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirm My Email Address</a><br><br>" +
                               "If you did not register for this account, please ignore this email.<br><br>" +
                               "Sincerely,<br>The AskHire Team";

            try
            {
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailMessage);
                _logger.LogInformation("Email confirmation link sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email confirmation link to {Email}. User registered but email verification email could not be sent.", user.Email);
                throw new ApplicationException("Registration successful, but we failed to send the email verification link. Please contact support.", ex);
            }

            _logger.LogInformation("User {Email} registered and assigned role {Role} successfully.", user.Email, user.Role);
            return user;
        }

        public async Task<bool> ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed: User with ID {UserId} not found.", userId);
                return false;
            }

            string decodedToken;
            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
                decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Email confirmation failed for user ID: {UserId}. Token decoding failed.", userId);
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                _logger.LogInformation("Email for user {Email} confirmed successfully.", user.Email);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Email confirmation failed for {Email} ({UserId}): {Code} - {Description}", user.Email, userId, error.Code, error.Description);
                }
            }
            return result.Succeeded;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(string refreshToken)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token validation failed. User: {UserEmail}, Provided Token: {ProvidedToken}, Stored Token: {StoredToken}, Expired: {IsExpired}",
                                   user?.Email ?? "N/A", refreshToken, user?.RefreshToken ?? "N/A", user?.RefreshTokenExpiryTime <= DateTime.UtcNow);
                return null;
            }

            return await CreateTokenResponse(user);
        }

        public async Task<bool> UpdateUserRoleAsync(UpdateRoleDto request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded) return false;

            var addResult = await _userManager.AddToRoleAsync(user, request.NewRole);
            if (!addResult.Succeeded) return false;

            user.Role = request.NewRole;
            await _userRepository.UpdateAsync(user);

            return true;
        }

        // --- New: Send Password Reset Email Method ---
        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Password reset request for non-existent email: {Email}", email);
                // For security, always return true even if user not found to prevent email enumeration attacks
                return true;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // The token needs to be URL-safe (Base64Url encoded)
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var frontendUrl = _configuration["FrontendUrl"];
            if (string.IsNullOrEmpty(frontendUrl))
            {
                _logger.LogError("FrontendUrl is not configured for password reset email link.");
                throw new InvalidOperationException("Frontend URL not configured for password reset.");
            }

            // The reset link will point to your frontend's Reset Password page
            var callbackUrl = QueryHelpers.AddQueryString($"{frontendUrl}/reset-password", new Dictionary<string, string?>
            {
                {"userId", user.Id.ToString()},
                {"token", encodedToken}
            });

            var emailSubject = "Reset Your Password for AskHire Account";
            var emailMessage = $"Dear {user.FirstName},<br><br>" +
                               "You have requested to reset your password. Please click the link below to set a new password:<br><br>" +
                               $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Reset My Password</a><br><br>" +
                               "This link is valid for a limited time. If you did not request a password reset, please ignore this email.<br><br>" +
                               "Sincerely,<br>The AskHire Team";
            try
            {
                await _emailService.SendEmailAsync(user.Email, emailSubject, emailMessage);
                _logger.LogInformation("Password reset link sent to {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
                // Propagate the email sending failure
                throw new ApplicationException($"Failed to send password reset email: {ex.Message}", ex);
            }
        }

        // --- New: Reset Password Method ---
        public async Task<bool> ResetPasswordAsync(Guid userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("Password reset failed: User with ID {UserId} not found for reset request.", userId);
                return false; // User not found (might be tampered link or user deleted)
            }

            string decodedToken;
            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
                decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Password reset failed for user ID: {UserId}. Token decoding failed.", userId);
                return false; // Token is not a valid base64url string
            }

            // Perform the password reset
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password successfully reset for user {Email}", user.Email);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Password reset failed for {Email}: {Code} - {Description}", user.Email, error.Code, error.Description);
                }
            }
            return result.Succeeded;
        }


        public bool IsAdult(string? dobString)
        {
            if (string.IsNullOrEmpty(dobString)) return false;

            if (!DateTime.TryParseExact(dobString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dobDate))
            {
                return false;
            }

            var today = DateTime.Today;
            var age = today.Year - dobDate.Year;
            if (dobDate.Date > today.AddYears(-age)) age--;

            return age >= 16 && age <= 100;
        }

        public bool IsRealName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            name = name.Trim();
            if (name.Length < 2 || name.Length > 50) return false;
            if (!name.Any(char.IsLetter)) return false;
            if (!char.IsLetter(name.First()) || !char.IsLetter(name.Last())) return false;

            for (int i = 0; i < name.Length - 1; i++)
            {
                char current = name[i];
                char next = name[i + 1];

                if ((current == '-' && next == '-') ||
                    (current == '\'' && next == '\'') ||
                    (current == '.' && next == '.') ||
                    (char.IsWhiteSpace(current) && char.IsWhiteSpace(next)))
                {
                    return false;
                }
            }

            for (int i = 0; i < name.Length; i++)
            {
                char current = name[i];
                if (current == '-' || current == '\'' || current == '.')
                {
                    if (i == name.Length - 1 && !char.IsLetter(current)) return false;
                    if (i < name.Length - 1)
                    {
                        char next = name[i + 1];
                        if (!char.IsLetter(next) && !char.IsWhiteSpace(next))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(15);
            await _userRepository.UpdateAsync(user);
            return refreshToken;
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString("D").ToUpper()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<User> GetUserFromTokenAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return null;

            return await _userRepository.GetByIdAsync(Guid.Parse(userId));
        }
    }
}