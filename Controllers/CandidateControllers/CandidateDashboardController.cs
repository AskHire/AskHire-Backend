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
            if (result == null || !result.Any())
                return NotFound("No applications found.");

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
