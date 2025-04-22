using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/candidates")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly UserService _userService;

        public CandidateController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/candidates
        [HttpGet]
        public async Task<IActionResult> GetCandidates()
        {
            var candidates = await _userService.GetCandidateAsync();
            return Ok(candidates);
        }

        // DELETE: api/candidates/{id}
        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteCandidate(Guid UserId)
        {
            var result = await _userService.DeleteCandidateAsync(UserId);
            if (!result)
            {
                return NotFound(new { message = "Candidate not found" });
            }

            return NoContent();
        }
    }
}
