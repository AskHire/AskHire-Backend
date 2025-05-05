using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerLonglistVacancyController : ControllerBase
    {
        private readonly IManagerLonglistIVacancyRepository _managerLonglistVacancyRepository;
        private readonly ILogger<ManagerLonglistVacancyController> _logger;

        public ManagerLonglistVacancyController(
            IManagerLonglistIVacancyRepository managerLonglistVacancyRepository,
            ILogger<ManagerLonglistVacancyController> logger)
        {
            _managerLonglistVacancyRepository = managerLonglistVacancyRepository;
            _logger = logger;
        }

        // GET: api/ManagerLonglistVacancy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vacancy>>> GetAllVacancies()
        {
            try
            {
                _logger.LogInformation("Getting all vacancies");
                var vacancies = await _managerLonglistVacancyRepository.GetAllVacanciesAsync();
                return Ok(vacancies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all vacancies");
                return StatusCode(500, new { message = "An error occurred while retrieving vacancies" });
            }
        }

        // GET: api/ManagerLonglistVacancy/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Vacancy>> GetVacancy(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting vacancy with ID: {Id}", id);
                var vacancy = await _managerLonglistVacancyRepository.GetVacancyByIdAsync(id);
                if (vacancy == null)
                {
                    _logger.LogWarning("Vacancy with ID {Id} not found", id);
                    return NotFound(new { message = $"Vacancy with ID {id} not found" });
                }
                return Ok(vacancy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vacancy with ID: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the vacancy" });
            }
        }

        // GET: api/ManagerLonglistVacancy/ByJobId/{jobId}
        [HttpGet("ByJobId/{jobId}")]
        public async Task<ActionResult<Vacancy>> GetVacancyByJobId(Guid jobId)
        {
            try
            {
                _logger.LogInformation("Getting vacancy with Job ID: {JobId}", jobId);
                var vacancy = await _managerLonglistVacancyRepository.GetVacancyByJobIdAsync(jobId);
                if (vacancy == null)
                {
                    _logger.LogWarning("Vacancy with Job ID {JobId} not found", jobId);
                    return NotFound(new { message = $"Vacancy with Job ID {jobId} not found" });
                }
                return Ok(vacancy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vacancy with Job ID: {JobId}", jobId);
                return StatusCode(500, new { message = "An error occurred while retrieving the vacancy" });
            }
        }
    }
}