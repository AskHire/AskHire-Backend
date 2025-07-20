using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.Extensions.Logging;
using System.Net; // For HttpStatusCode
using Microsoft.AspNetCore.WebUtilities; // Required for QueryHelpers
using System.Text.Encodings.Web; // Required for HtmlEncoder
using Microsoft.Extensions.Configuration; // Required for IConfiguration
using Microsoft.IdentityModel.Tokens; // For SecurityTokenExpiredException

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthController> _logger;
    private readonly string _frontendUrl;
    private readonly string _backendBaseUrl; // Added for clarity, though it's also available via IConfiguration

    public AuthController(IAuthService authService, UserManager<User> userManager, ILogger<AuthController> logger, IConfiguration configuration)
    {
        _authService = authService;
        _userManager = userManager;
        _logger = logger;
        _frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:3000";
        _backendBaseUrl = configuration["BackendBaseUrl"] ?? "http://localhost:5190"; // Assign backend base URL
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserRegisterDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(request.DOB) && !_authService.IsAdult(request.DOB))
            {
                ModelState.AddModelError("DOB", "You must be at least 16 years old and no older than 100.");
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(existingUser))
                {
                    _logger.LogInformation("Attempted to register with existing unconfirmed email: {Email}.", request.Email);
                    return BadRequest(new { message = "An account with this email already exists but is not confirmed. Please check your email or try logging in to resend the verification." });
                }
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return BadRequest(ModelState);
            }

            var user = await _authService.RegisterAsync(request);
            if (user is null)
            {
                _logger.LogWarning("Registration failed for email {Email} due to internal Identity error or other service issue.", request.Email);
                return BadRequest(new { message = "Registration failed. Please check your inputs and try again." });
            }

            return Ok(new { message = "Registration successful! Please check your email to confirm your account." });
        }
        catch (ApplicationException appEx)
        {
            _logger.LogError(appEx, "Application error during user registration for email: {Email}. Message: {Message}", request.Email, appEx.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = appEx.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during user registration for email: {Email}", request.Email);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred during registration. Please try again later." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                return BadRequest(new { message = "Invalid email or password." });
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set to true in production (HTTPS)
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(3)
            };
            Response.Cookies.Append("jwt", result.AccessToken, cookieOptions);

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set to true in production (HTTPS)
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(15)
            };
            Response.Cookies.Append("refreshToken", result.RefreshToken, refreshTokenOptions);

            var user = await _authService.GetUserFromTokenAsync(result.AccessToken);
            _logger.LogInformation("User {Email} logged in successfully.", user?.Email);
            return Ok(new
            {
                message = "Login successful",
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                userEmail = user?.Email,
                userRole = user?.Role
            });
        }
        catch (ApplicationException appEx)
        {
            _logger.LogWarning(appEx, "Login failed for {Email}: {Message}", request.Email, appEx.Message);
            return BadRequest(new { message = appEx.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during user login for email: {Email}", request.Email);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred during login. Please try again later." });
        }
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
    {
        try
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Invalid email confirmation request: Missing userId or token.");
                return Redirect($"{_frontendUrl}/verify-email-status?status=failure&message=Invalid verification link.");
            }

            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (result)
            {
                _logger.LogInformation("Email confirmed successfully for user ID: {UserId}", userId);
                return Redirect($"{_frontendUrl}/verify-email-status?status=success&message=Your email has been successfully confirmed. You can now log in.");
            }
            else
            {
                _logger.LogWarning("Email confirmation failed for user ID: {UserId}. Token might be invalid or expired.", userId);
                return Redirect($"{_frontendUrl}/verify-email-status?status=failure&message=Email verification failed. The link might be invalid or expired. Please try registering again or contact support.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email for user ID: {UserId}", userId);
            return Redirect($"{_frontendUrl}/verify-email-status?status=error&message=An unexpected error occurred during email verification. Please try again later.");
        }
    }

    // --- New Endpoint: Request Password Reset Link ---
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Always return a success message for security, even if email not found,
            // to prevent revealing whether an email is registered or not.
            var result = await _authService.SendPasswordResetEmailAsync(request.Email);

            // The actual result of SendPasswordResetEmailAsync is logged internally for security,
            // but the API always returns a successful message to the client.
            _logger.LogInformation("Password reset request received for email {Email}. Email sending result: {Result}", request.Email, result ? "Success" : "Failure (logged internally)");

            return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
        }
        catch (ApplicationException appEx)
        {
            _logger.LogError(appEx, "Application error sending password reset email to {Email}. Message: {Message}", request.Email, appEx.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = appEx.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing forgot password request for email: {Email}", request.Email);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred. Please try again later." });
        }
    }

    // --- New Endpoint: Reset Password ---
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResetPasswordAsync(request.UserId, request.Token, request.NewPassword);

            if (result)
            {
                _logger.LogInformation("Password successfully reset for user ID: {UserId}", request.UserId);
                return Ok(new { message = "Password has been reset successfully. You can now log in with your new password." });
            }
            else
            {
                _logger.LogWarning("Password reset failed for user ID: {UserId}. Token might be invalid or expired, or new password invalid.", request.UserId);
                return BadRequest(new { message = "Password reset failed. The link might be invalid or expired, or your new password does not meet requirements. Please try again." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user ID: {UserId}", request.UserId);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred while resetting your password. Please try again later." });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken()
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                Debug.WriteLine("No refresh token in cookies");
                return Unauthorized(new { message = "No refresh token provided. Please log in again." });
            }

            var result = await _authService.RefreshTokensAsync(refreshToken);
            if (result == null)
            {
                Debug.WriteLine("Invalid refresh token");
                Response.Cookies.Delete("jwt", new CookieOptions { HttpOnly = true, Secure = false, SameSite = SameSiteMode.Lax, Path = "/" });
                Response.Cookies.Delete("refreshToken", new CookieOptions { HttpOnly = true, Secure = false, SameSite = SameSiteMode.Lax, Path = "/" });
                return Unauthorized(new { message = "Your session has expired or the refresh token is invalid. Please log in again." });
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(3)
            };
            Response.Cookies.Append("jwt", result.AccessToken, cookieOptions);

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(15)
            };
            Response.Cookies.Append("refreshToken", result.RefreshToken, refreshCookieOptions);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during token refresh.");
            return StatusCode(500, new { message = "An unexpected error occurred during token refresh. Please try again later." });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update-role")]
    public async Task<IActionResult> UpdateUserRole(UpdateRoleDto request)
    {
        try
        {
            if (request.UserId == Guid.Empty || string.IsNullOrEmpty(request.NewRole))
            {
                return BadRequest(new { message = "User ID and New Role are required." });
            }

            var result = await _authService.UpdateUserRoleAsync(request);
            if (!result)
                return BadRequest(new { message = "Failed to update user role or user not found." });

            return Ok(new { message = "User role updated successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during user role update for user ID: {UserId}", request.UserId);
            return StatusCode(500, new { message = "An unexpected error occurred while updating user role. Please try again later." });
        }
    }

    [Authorize]
    [HttpGet]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        try
        {
            return Ok("You are authenticated!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in AuthenticatedOnlyEndpoint.");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnlyEndpoint()
    {
        try
        {
            return Ok("Admin");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in AdminOnlyEndpoint.");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        }
    }

    [Authorize(Roles = "Manager")]
    [HttpGet("manager-only")]
    public IActionResult ManagerOnlyEndpoint()
    {
        try
        {
            return Ok("Manager");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in ManagerOnlyEndpoint.");
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        try
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };

            Response.Cookies.Delete("jwt", cookieOptions);
            Response.Cookies.Delete("refreshToken", cookieOptions);

            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during user logout.");
            return StatusCode(500, new { message = "An unexpected error occurred during logout. Please try again later." });
        }
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            var token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader != null && authHeader.StartsWith("Bearer "))
                {
                    token = authHeader.Substring("Bearer ".Length);
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserFromTokenAsync(token);
            if (user == null) return NotFound(new { message = "User not found or invalid token." });

            return Ok(new
            {
                user.Email,
                Id = user.Id.ToString("D").ToUpper(),
                user.Role
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user details from token.");
            return StatusCode(500, new { message = "An unexpected error occurred while fetching user data. Please try again later." });
        }
    }
}