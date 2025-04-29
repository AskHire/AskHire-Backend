using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminUserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AdminUserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/AdminUser
        [HttpGet]
       
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        // PUT: api/AdminUser/UpdateRole
        [HttpPut("UpdateRole")]
        
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRoleDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return NotFound("User not found.");

            user.Role = request.NewRole;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest("Failed to update role.");

            return Ok(new { message = "User role updated successfully." });
        }


        [Authorize(Roles = "Admin")] // Only Admins can delete users
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest("Failed to delete user.");

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
