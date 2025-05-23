using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Models.DTOs;


[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public ProfileController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("get-profile/{userId}")]
    public async Task<IActionResult> GetProfile(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound("User not found.");

        var dto = new UpdateProfileDto
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MobileNumber = user.MobileNumber,
            Gender = user.Gender,
            Address = user.Address,
            NIC = user.NIC,
            DOB = user.DOB
        };

        return Ok(dto);
    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user == null) return NotFound("User not found.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.MobileNumber = dto.MobileNumber;
        user.Gender = dto.Gender;
        user.Address = dto.Address;
        user.NIC = dto.NIC;
        user.DOB = dto.DOB;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest("Failed to update profile.");

        return Ok("Profile updated.");
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user == null) return NotFound("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Password changed.");
    }
}
