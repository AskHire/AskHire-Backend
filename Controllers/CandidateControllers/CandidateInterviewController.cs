using AskHire_Backend.Interfaces.Services.ICandidateServices;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers.CandidateControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateInterviewController : ControllerBase
    {
        private readonly ICandidateInterviewService _interviewService;

        public CandidateInterviewController(ICandidateInterviewService interviewService)
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