using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System; // Required for Guid and DateTime
using Microsoft.Extensions.Logging; // For logging errors

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthController> _logger; // Inject ILogger

    public AuthController(IAuthService authService, UserManager<User> userManager, ILogger<AuthController> logger)
    {
        _authService = authService;
        _userManager = userManager;
        _logger = logger; // Initialize logger
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

            var user = await _authService.RegisterAsync(request);
            if (user is null)
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return BadRequest(ModelState);
                }
                return BadRequest(new { message = "Registration failed. Please check your inputs and try again." });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during user registration for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An unexpected error occurred during registration. Please try again later." });
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
            return Ok(new
            {
                message = "Login successful",
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                userEmail = user?.Email,
                userRole = user?.Role
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during user login for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An unexpected error occurred during login. Please try again later." });
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
                return Unauthorized("Invalid refresh token.");
            }

            var result = await _authService.RefreshTokensAsync(refreshToken);
            if (result == null)
            {
                Debug.WriteLine("Invalid refresh token");
                return Unauthorized("Invalid refresh token.");
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