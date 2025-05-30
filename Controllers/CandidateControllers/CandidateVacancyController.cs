using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateVacancyController : ControllerBase
    {
        private readonly ICandidateVacancyService _candidateVacancyService;

        public CandidateVacancyController(ICandidateVacancyService candidateVacancyService)
        {
            _candidateVacancyService = candidateVacancyService;
        }

        // GET: api/CandidateVacancy/JobWiseVacancies
        [HttpGet("JobWiseVacancies")]
        public async Task<ActionResult<IEnumerable<CandidateVacancyDto>>> GetJobWiseVacancies()
        {
            var vacancies = await _candidateVacancyService.GetJobWiseVacanciesAsync();
            return Ok(vacancies);
        }

        [HttpGet("MostApplied")]
        public async Task<ActionResult<IEnumerable<CandidateVacancyDto>>> GetMostAppliedVacancies()
        {
            var vacancies = await _candidateVacancyService.GetMostAppliedVacanciesAsync();
            return Ok(vacancies);
        }

        [HttpGet("Latest")]
        public async Task<ActionResult<IEnumerable<CandidateVacancyDto>>> GetLatestVacancies()
        {
            var vacancies = await _candidateVacancyService.GetLatestVacanciesAsync();
            return Ok(vacancies);
        }

        [HttpGet("{vacancyId}")]
        public async Task<ActionResult<CandidateJobShowDto>> GetVacancyById(Guid vacancyId)
        {
            var vacancy = await _candidateVacancyService.GetVacancyByIdAsync(vacancyId);
            if (vacancy == null)
            {
                return NotFound();
            }
            return Ok(vacancy);
        }


    }
}
