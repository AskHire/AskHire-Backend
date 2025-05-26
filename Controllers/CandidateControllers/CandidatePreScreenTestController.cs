using AskHire_Backend.Interfaces.Services.ICandidateServices;
using AskHire_Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers.CandidateControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatePreScreenTestController : ControllerBase
    {
        private readonly ICandidatePreScreenTestService _service;

        public CandidatePreScreenTestController(ICandidatePreScreenTestService service)
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

