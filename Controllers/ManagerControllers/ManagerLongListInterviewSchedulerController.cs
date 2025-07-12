using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.DTOs.ManagerDTOs;
using AskHire_Backend.Interfaces.Services.IManagerServices;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers.Manager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerLongListInterviewSchedulerController : ControllerBase
    {
        private readonly IManagerLongListInterviewService _longListInterviewService;
        private readonly ILogger<ManagerLongListInterviewSchedulerController> _logger;
        

        public ManagerLongListInterviewSchedulerController(
            IManagerLongListInterviewService longListInterviewService,
            ILogger<ManagerLongListInterviewSchedulerController> logger)
        {
            _longListInterviewService = longListInterviewService;
            _logger = logger;
        }
       





        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleLongListInterviews([FromBody] ManagerLongListInterviewScheduleRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model: {@ModelState}", ModelState);
                return BadRequest(new { message = "Invalid request data", errors = ModelState });
            }

            try
            {
                // Parse dates for validation but let the service handle the details
                DateTime startTime, endTime;
                TimeSpan duration;

                if (!DateTime.TryParse(request.StartTime, out startTime))
                {
                    return BadRequest(new { message = "Invalid Start Time format." });
                }

                if (!DateTime.TryParse(request.EndTime, out endTime))
                {
                    return BadRequest(new { message = "Invalid End Time format." });
                }

                if (!TimeSpan.TryParse(request.InterviewDuration, out duration))
                {
                    return BadRequest(new { message = "Invalid Interview Duration format." });
                }

                if (startTime >= endTime)
                {
                    return BadRequest(new { message = "Start time must be before end time." });
                }

                // Log whether emails will be sent
                _logger.LogInformation("Scheduling interviews with email sending set to: {SendEmail}", request.SendEmail);

                var result = await _longListInterviewService.ScheduleLongListInterviewsAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new
                {
                    message = result.Message,
                    scheduledCount = result.ScheduledCount,
                    notScheduledCount = result.NotScheduledCount,
                    emailsSent = request.SendEmail
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling interviews");
                return StatusCode(500, new { message = "An error occurred while scheduling interviews." });
            }
        }

        [HttpGet("unscheduled-candidates/{vacancyId}")]
        public async Task<IActionResult> GetUnscheduledCandidates(string vacancyId)
        {
            try
            {
                _logger.LogInformation("Getting unscheduled candidates for vacancy ID: {VacancyId}", vacancyId);

                // Validate and parse the GUID
                if (!Guid.TryParse(vacancyId, out Guid vacancyGuid))
                {
                    return BadRequest(new { message = "Invalid vacancy ID format." });
                }

                var candidates = await _longListInterviewService.GetUnscheduledCandidatesAsync(vacancyGuid);
                _logger.LogInformation("Retrieved {Count} unscheduled candidates", candidates.Count);

                return Ok(candidates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching unscheduled candidates for vacancy ID: {VacancyId}", vacancyId);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("scheduled-interviews/{vacancyId}")]
        public async Task<IActionResult> GetScheduledInterviews(string vacancyId)
        {
            try
            {
                _logger.LogInformation("Getting scheduled interviews for vacancy ID: {VacancyId}", vacancyId);

                // Validate and parse the GUID
                if (!Guid.TryParse(vacancyId, out Guid vacancyGuid))
                {
                    return BadRequest(new { message = "Invalid vacancy ID format." });
                }

                var interviews = await _longListInterviewService.GetScheduledInterviewsAsync(vacancyGuid);
                return Ok(interviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching scheduled interviews for vacancy ID: {VacancyId}", vacancyId);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }
}