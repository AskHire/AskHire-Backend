// Controllers/ManagerCandidatesController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using AskHire_Backend.Interfaces.Services.IManagerServices;

namespace AskHire_Backend.Controllers.Manager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerCandidatesController : ControllerBase
    {
        private readonly IManagerCandidateService _candidateService;
        private readonly ICandidateFileService _fileService;

        public ManagerCandidatesController(IManagerCandidateService candidateService, ICandidateFileService fileService)
        {
            _candidateService = candidateService;
            _fileService = fileService;
        }

        // GET: api/ManagerCandidates - Get all LongList candidates
        [HttpGet]
        public async Task<IActionResult> GetAllLongListCandidates()
        {
            try
            {
                // Get all applications with LongList status
                var allApplications = await _candidateService.GetApplicationsByStatusAsync("LongList");
                return Ok(allApplications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        // GET: api/ManagerCandidates/{id}
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
                // First get all candidates for this vacancy
                var allCandidates = await _candidateService.GetApplicationsByVacancyNameAsync(vacancyName);

                // Define a function that safely checks for the status property using dynamic
                Func<dynamic, bool> isLongList = candidate => {
                    try
                    {
                        string status = null;

                        // Try to access status property (case-insensitive)
                        var candidateType = candidate.GetType();
                        var statusProperty = candidateType.GetProperty("status") ?? candidateType.GetProperty("Status");

                        if (statusProperty != null)
                        {
                            var statusValue = statusProperty.GetValue(candidate);
                            status = statusValue?.ToString()?.ToLower();
                        }

                        return status == "longlist";
                    }
                    catch
                    {
                        return false;
                    }
                };

                // Filter to only get those with "LongList" status
                var longListCandidates = allCandidates
                    .Where(c => isLongList((dynamic)c))
                    .ToList();

                return Ok(longListCandidates);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception details here if you have a logger
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpGet("download-cv/{applicationId}")]
        public async Task<IActionResult> DownloadCV(Guid applicationId)
        {
            try
            {
                // Use the file service to download CV
                return await _fileService.DownloadCvAsync(applicationId);
            }
            catch (Exception ex)
            {
                // Log the exception details here if you have a logger
                return StatusCode(500, $"An error occurred while downloading the CV: {ex.Message}");
            }
        }

        // GET: api/ManagerCandidates/status/{status}
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

        // GET: api/ManagerCandidates/statistics
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

        // GET: api/ManagerCandidates/statistics/vacancy/{vacancyId}
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