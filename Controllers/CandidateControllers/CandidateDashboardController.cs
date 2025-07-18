using AskHire_Backend.DTOs;
using AskHire_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers.CandidateControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateDashboardController : ControllerBase
    {
        private readonly ICandidateDashboardService _service;

        public CandidateDashboardController(ICandidateDashboardService service)
        {
            _service = service;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCandidateApplications(Guid userId)
        {
            var result = await _service.GetCandidateApplicationsAsync(userId);

            // RECOMMENDED CHANGE: Return 200 OK with an empty list if no applications are found
            // A 404 Not Found usually means the resource itself (like the user ID in the path) wasn't found,
            // not that the resource exists but has no related items.
            if (result == null || !result.Any())
            {
                return Ok(new List<CandidateDashboardDto>()); // Return an empty list
            }

            return Ok(result);
        }

        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> DeleteCandidateApplication(Guid applicationId)
        {
            var success = await _service.DeleteCandidateApplicationAsync(applicationId);
            if (!success)
                return NotFound("Application not found or could not be deleted.");

            return NoContent();
        }
    }
}