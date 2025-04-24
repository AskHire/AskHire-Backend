// Reorganized standard architecture for AdminUsersController using Repositories and Services

using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Repositories.Interfaces;
using AskHire_Backend.Services.Interfaces;
using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Controllers
{
    [Route("api/adminusers")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _userService;

        public AdminUsersController(IAdminUserService userService)
        {
            _userService = userService;
        }

        // General Users
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("total-users")]
        public async Task<ActionResult<int>> GetTotalUsers()
        {
            try
            {
                var count = await _userService.GetTotalUsersAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });

            }
        }



        [HttpPost("users")]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, User user)
        {
            var result = await _userService.UpdateUserAsync(id, user);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPatch("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] string newRole)
        {
            var result = await _userService.UpdateUserRoleAsync(id, newRole);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // Admins
        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            return Ok(await _userService.GetAdminsAsync());
        }

        [HttpDelete("admins/{UserId}")]
        public async Task<IActionResult> DeleteAdmin(Guid UserId)
        {
            var result = await _userService.DeleteAdminAsync(UserId);
            if (!result) return NotFound(new { message = "Admin not found" });
            return NoContent();
        }

        // Candidates
        [HttpGet("candidates")]
        public async Task<IActionResult> GetCandidates()
        {
            return Ok(await _userService.GetCandidatesAsync());
        }

        [HttpDelete("candidates/{UserId}")]
        public async Task<IActionResult> DeleteCandidate(Guid UserId)
        {
            var result = await _userService.DeleteCandidateAsync(UserId);
            if (!result) return NotFound(new { message = "Candidate not found" });
            return NoContent();
        }

        // Managers
        [HttpGet("managers")]
        public async Task<IActionResult> GetManagers()
        {
            return Ok(await _userService.GetManagersAsync());
        }

        [HttpDelete("managers/{UserId}")]
        public async Task<IActionResult> DeleteManager(Guid UserId)
        {
            var result = await _userService.DeleteManagerAsync(UserId);
            if (!result) return NotFound(new { message = "Manager not found" });
            return NoContent();
        }
    }
}
