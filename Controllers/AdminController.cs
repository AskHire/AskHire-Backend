using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserService _userService;

        public AdminController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/admins
        [HttpGet]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _userService.GetAdminsAsync();
            return Ok(admins);
        }

        // DELETE: api/admins/{id}
        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteAdmin(Guid UserId)
        {
            var result = await _userService.DeleteAdminAsync(UserId);
            if (!result)
            {
                return NotFound(new { message = "Admin not found" });
            }

            return NoContent();
        }
    }
}
