using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers.Manager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet("interview-status-summary")]
        public async Task<IActionResult> GetInterviewStatusSummary()
        {
            var summary = await _applicationService.GetInterviewStatusSummaryAsync();
            return Ok(summary);
        }
    }
}
