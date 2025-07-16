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
using System.Linq; // For .Any() on strings

namespace AskHire_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(IAuthRepository userRepository, IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        public async Task<User?> RegisterAsync(UserRegisterDto request)
        {
            if (await _userRepository.AnyAsync(request.Email))
            {
                return null;
            }

            if (!IsAdult(request.DOB))
            {
                return null;
            }

            // The IsRealName checks are now primarily in the controller, but you can keep them here
            // if you want this service to return a more specific error than just 'null' for name issues,
            // which would then be handled in the controller. For now, the controller directly calls IsRealName.

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
                ProfilePictureUrl = "/avatars/avatar2.png" // Assuming a default for new users
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return null;
            }

            await _userManager.AddToRoleAsync(user, user.Role);

            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(string refreshToken)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
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

        // Custom "Real Name" Validation Logic (improved)
        public bool IsRealName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false; // Name cannot be empty or just whitespace
            }

            name = name.Trim(); // Trim leading/trailing whitespace

            // Check length again (redundant with DTO, but safe)
            if (name.Length < 2 || name.Length > 50)
            {
                return false;
            }

            // Ensure it contains at least one letter (to prevent names like "--" or "''")
            if (!name.Any(char.IsLetter))
            {
                return false;
            }

            // Ensure it starts and ends with a letter. This is a common requirement for names.
            if (!char.IsLetter(name.First()) || !char.IsLetter(name.Last()))
            {
                return false;
            }

            // Prevent multiple consecutive hyphens, apostrophes, or periods, and also multiple spaces.
            // This is to catch things like "John--Doe", "O''Malley", "Mr..", "First  Last"
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

            // Additional check for name fragments: a hyphen, apostrophe, or period must be followed by a letter or space.
            // This prevents names ending in a symbol or having isolated symbols like "John-", "O'", "Smith."
            for (int i = 0; i < name.Length; i++)
            {
                char current = name[i];
                if (current == '-' || current == '\'' || current == '.')
                {
                    // If it's the last character and not a letter (already caught by name.Last() check)
                    if (i == name.Length - 1 && !char.IsLetter(current)) return false;
                    
                    // If it's not the last character, check the next one
                    if (i < name.Length - 1) {
                        char next = name[i + 1];
                        if (!char.IsLetter(next) && !char.IsWhiteSpace(next))
                        {
                            return false; // Symbol must be followed by a letter or space.
                        }
                    }
                }
            }
            
            return true; // If all checks pass
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