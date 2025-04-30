using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerCheckController : ControllerBase
    {
        private readonly IAnswerCheckService _service;

        public AnswerCheckController(IAnswerCheckService service)
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

