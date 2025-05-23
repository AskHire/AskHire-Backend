// Controllers/CandidatesController.cs
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

        public ManagerCandidatesController(IManagerCandidateService candidateService)
        {
            _candidateService = candidateService;
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
                // First get all candidates for this vacancy
                var allCandidates = await _candidateService.GetApplicationsByVacancyNameAsync(vacancyName);

                // Define a function that safely checks for the status property using dynamic
                Func<dynamic, bool> isLongList = candidate => {
                    try
                    {
                        string status = null;

                        // Try to access status property (case-insensitive)
                        if (candidate.GetType().GetProperty("status") != null)
                            status = candidate.status?.ToString()?.ToLower();
                        else if (candidate.GetType().GetProperty("Status") != null)
                            status = candidate.Status?.ToString()?.ToLower();

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
                // Check if the application ID is valid
                if (applicationId == Guid.Empty)
                {
                    return BadRequest("Invalid application ID");
                }

                var fileBytes = await _candidateService.GetCVFileAsync(applicationId);

                // Check if the file bytes are null or empty
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return NotFound($"CV file for application ID {applicationId} is empty or not found");
                }

                var fileName = await _candidateService.GetCVFileNameAsync(applicationId);

                // Check if the filename is valid
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"CV_{applicationId}.pdf";
                }

                var contentType = "application/pdf";

                // Try to determine content type from file extension if not pdf
                if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var extension = Path.GetExtension(fileName).ToLowerInvariant();
                    switch (extension)
                    {
                        case ".docx":
                            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            break;
                        case ".doc":
                            contentType = "application/msword";
                            break;
                            // Add more content types as needed
                    }
                }

                return File(fileBytes, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the specific exception details
                // If you have a logger, uncomment and use the following line:
                // _logger.LogError(ex, "Error downloading CV for application ID: {ApplicationId}", applicationId);

                return StatusCode(500, $"An error occurred while downloading the CV: {ex.Message}");
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