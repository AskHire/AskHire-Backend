using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<List<UserInterviewDetailsDto>>> GetInterviewsByUserId(Guid userId)
        {
            var interviews = await _interviewService.GetInterviewsByUserIdAsync(userId);
            if (interviews == null || interviews.Count == 0)
            {
                return NotFound("No interviews found for the given user ID.");
            }

            return Ok(interviews);
        }
    }
}
