using AskHire_Backend.Models.Entities;
using AskHire_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacancyController : ControllerBase
    {
        private readonly IVacancyService _vacancyService;
        private readonly ILogger<VacancyController> _logger;

        public VacancyController(IVacancyService vacancyService, ILogger<VacancyController> logger)
        {
            _vacancyService = vacancyService ?? throw new ArgumentNullException(nameof(vacancyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<ActionResult<Vacancy>> CreateVacancy([FromBody] Vacancy vacancy)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdVacancy = await _vacancyService.CreateVacancyAsync(vacancy);
                return CreatedAtAction(nameof(GetVacancyById), new { id = createdVacancy.VacancyId }, createdVacancy);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Null vacancy data provided");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating vacancy");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVacancyById(Guid id)
        {
            try
            {
                var vacancy = await _vacancyService.GetVacancyByIdAsync(id);
                if (vacancy == null)
                {
                    _logger.LogWarning($"Vacancy with ID {id} not found");
                    return NotFound(new { message = "Vacancy not found" });
                }
                return Ok(vacancy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vacancy with ID {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVacancies()
        {
            try
            {
                var vacancies = await _vacancyService.GetAllVacanciesAsync();
                return Ok(vacancies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all vacancies");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVacancy(Guid id, [FromBody] Vacancy vacancy)
        {
            if (id != vacancy.VacancyId)
            {
                _logger.LogWarning($"Vacancy ID mismatch: URL ID={id}, Body ID={vacancy.VacancyId}");
                return BadRequest(new { message = "Vacancy ID mismatch." });
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedVacancy = await _vacancyService.UpdateVacancyAsync(vacancy);
                if (updatedVacancy == null)
                {
                    _logger.LogWarning($"Vacancy with ID {id} not found for update");
                    return NotFound(new { message = "Vacancy not found." });
                }
                return Ok(updatedVacancy);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Null vacancy data provided for update");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating vacancy with ID {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacancy(Guid id)
        {
            try
            {
                var success = await _vacancyService.DeleteVacancyAsync(id);
                if (!success)
                {
                    _logger.LogWarning($"Vacancy with ID {id} not found for deletion");
                    return NotFound(new { message = "Vacancy not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting vacancy with ID {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
