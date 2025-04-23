// Controllers/CandidatesController.cs
using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidatesController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        // GET: api/Candidates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetApplications()
        {
            try
            {
                var applications = await _candidateService.GetAllApplicationsAsync();
                return Ok(applications);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Candidates/{id}
        [HttpGet("{applicationId}")]
        public async Task<ActionResult<object>> GetApplicationById(Guid applicationId)
        {
            try
            {
                var application = await _candidateService.GetApplicationByIdAsync(applicationId);

                if (application == null)
                {
                    return NotFound(); // Return 404 if no candidate found
                }

                return Ok(application); // Return 200 OK with the application details
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("vacancies")]
        public async Task<IActionResult> GetVacancies()
        {
            try
            {
                var vacancies = await _candidateService.GetVacanciesAsync(); // Call service method to get vacancies
                return Ok(vacancies); // Return the list of vacancies
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("vacancy/{vacancyName}")]
        public async Task<IActionResult> GetCandidatesByVacancy(string vacancyName)
        {
            try
            {
                var applications = await _candidateService.GetApplicationsByVacancyNameAsync(vacancyName);
                return Ok(applications);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }




        [HttpGet("download-cv/{applicationId}")]
        public async Task<IActionResult> DownloadCV(Guid applicationId)
        {
            try
            {
                var fileBytes = await _candidateService.GetCVFileAsync(applicationId);
                var fileName = await _candidateService.GetCVFileNameAsync(applicationId);
                var contentType = "application/pdf";

                return File(fileBytes, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Candidates/status/{status}
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetCandidatesByStatus(string status)
        {
            try
            {
                var applications = await _candidateService.GetApplicationsByStatusAsync(status);
                return Ok(applications);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Candidates/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetCandidateStatistics()
        {
            try
            {
                var statistics = await _candidateService.GetStatisticsAsync();
                return Ok(statistics);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Candidates/statistics/vacancy/{vacancyId}
        [HttpGet("statistics/vacancy/{vacancyId}")]
        public async Task<IActionResult> GetCandidateStatisticsByVacancy(Guid vacancyId)
        {
            try
            {
                var statistics = await _candidateService.GetStatisticsByVacancyAsync(vacancyId);
                return Ok(statistics);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



    }
}