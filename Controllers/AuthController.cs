using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserRegisterDto request)
    {
        var user = await _authService.RegisterAsync(request);
        if (user is null)
            return BadRequest("Username already exists.");

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
            return BadRequest("Invalid username or password.");

        // Store raw token values to ensure DB and cookies match exactly
        var accessToken = result.AccessToken;
        var refreshToken = result.RefreshToken;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // HTTP for local dev
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(3)
        };
        Response.Cookies.Append("jwt", accessToken, cookieOptions);

        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTime.UtcNow.AddMinutes(15)
        };
        Response.Cookies.Append("refreshToken", refreshToken, refreshTokenOptions);



        return Ok(new
        {
            message = "Login successful",
            accessToken = accessToken,
            refreshToken = refreshToken
        });
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken()
    {
        // Get refresh token from cookie
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            Debug.WriteLine("No refresh token in cookies");
            return Unauthorized("Invalid refresh token.");
        }

        // Call service with just the refresh token
        var result = await _authService.RefreshTokensAsync(refreshToken);
        if (result == null)
        {
            Debug.WriteLine("Invalid refresh token");
            return Unauthorized("Invalid refresh token.");
        }

        // Set new cookies
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

    [Authorize(Roles = "Admin")]
    [HttpPut("update-role")]
    public async Task<IActionResult> UpdateUserRole(UpdateRoleDto request)
    {
        var result = await _authService.UpdateUserRoleAsync(request);
        if (!result)
            return BadRequest("Failed to update user role.");

        return Ok("User role updated successfully.");
    }

    [Authorize]
    [HttpGet]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        return Ok("You are authenticated!");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnlyEndpoint()
    {
        return Ok("Admin");
    }

    [Authorize(Roles = "Manager")]
    [HttpGet("manager-only")]
    public IActionResult ManagerOnlyEndpoint()
    {
        return Ok("Manager");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
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

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        // Get token from cookie or from Authorization header
        var token = Request.Cookies["jwt"];

        // If no token in cookie, check Authorization header
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
            return Unauthorized(); // No token found
        }

        var user = await _authService.GetUserFromTokenAsync(token);
        if (user == null) return NotFound();

        // Convert the user.Id to a string and ensure it's uppercase
        return Ok(new
        {
            user.Email,
            Id = user.Id.ToString().ToUpper(),  // Ensure the GUID is in uppercase
            user.Role
        });
    }
}