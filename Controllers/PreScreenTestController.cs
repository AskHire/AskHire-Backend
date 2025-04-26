using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreScreenTestController : ControllerBase
    {
        private readonly IPreScreenTestService _service;

        public PreScreenTestController(IPreScreenTestService service)
        {
            _service = service;
        }

        [HttpGet("{applicationId}")]
        public async Task<ActionResult<PreScreenTestDto>> GetPreScreenVacancyInfo(Guid applicationId)
        {
            var result = await _service.GetVacancyInfo(applicationId);
            if (result == null)
                return NotFound("Application or related Vacancy not found.");

            return Ok(result);
        }

        [HttpGet("Questions/{applicationId}")]
        public async Task<ActionResult<PreScreenTestDto>> GetPreScreenQuestions(Guid applicationId)
        {
            var result = await _service.GetQuestions(applicationId);
            if (result == null)
                return NotFound("Application, related Vacancy, or Job Role not found.");

            return Ok(result);
        }
    }
}

