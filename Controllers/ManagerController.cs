using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/managers")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly UserService _userService;

        public ManagerController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/managers
        [HttpGet]
        public async Task<IActionResult> GetManagers()
        {
            var managers = await _userService.GetManagerAsync();
            return Ok(managers);
        }

        // DELETE: api/managers/{id}
        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteManager(Guid UserId)
        {
            var result = await _userService.DeleteManagerAsync(UserId);
            if (!result)
            {
                return NotFound(new { message = "Manager not found" });
            }

            return NoContent();
        }
    }
}
