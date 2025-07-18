using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Interfaces.Services;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _service;

    public ProfileController(IProfileService service) => _service = service;

    [Authorize]
    [HttpGet]

    public async Task<IActionResult> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _service.GetProfileAsync(userId);
        if (user == null) return NotFound();

        // Set default avatar if none exists
        var profilePictureUrl = string.IsNullOrWhiteSpace(user.ProfilePictureUrl)
            ? null
            : user.ProfilePictureUrl;

        return Ok(new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            user.Gender,
            user.DOB,
            user.MobileNumber,
            user.Address,
            user.Role,
            ProfilePictureUrl = profilePictureUrl,
            user.SignUpDate
        });
    }


    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.UpdateProfileAsync(userId, dto);
        return result ? Ok("Updated") : BadRequest("Update failed");
    }

    [Authorize]
    [HttpPost("set-avatar")]
    public async Task<IActionResult> SetAvatar([FromBody] string avatarFileName)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _service.SetAvatarAsync(userId, avatarFileName);
        return success ? Ok(new { imageUrl = $"/avatars/{avatarFileName}" }) : BadRequest("Invalid avatar");
    }
}

