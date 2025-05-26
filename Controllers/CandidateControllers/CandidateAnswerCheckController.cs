using AskHire_Backend.Interfaces.Services.ICandidateServices;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateAnswerCheckController : ControllerBase
    {
        private readonly ICandidateAnswerCheckService _service;

        public CandidateAnswerCheckController(ICandidateAnswerCheckService service)
        {
            _service = service;
        }

        [HttpPost("mcq/{applicationId}")]
        public async Task<IActionResult> CheckAnswers(Guid applicationId, [FromBody] CheckAnswersRequest request)
        {
            var result = await _service.CheckAnswersAsync(applicationId, request);

            if (!string.IsNullOrEmpty(result.Error))
            {
                return BadRequest(result.Error);
            }

            return Ok(result);
        }
    }
}

