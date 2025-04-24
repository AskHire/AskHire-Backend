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
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;
        private readonly ILogger<ReminderController> _logger;

        public ReminderController(IReminderService reminderService, ILogger<ReminderController> logger)
        {
            _reminderService = reminderService ?? throw new ArgumentNullException(nameof(reminderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<ActionResult<Reminder>> CreateReminder([FromBody] Reminder reminder)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdReminder = await _reminderService.CreateReminderAsync(reminder);
                return CreatedAtAction(nameof(GetReminderById), new { id = createdReminder.ReminderId }, createdReminder);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Null reminder data provided");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating reminder");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReminderById(Guid id)
        {
            try
            {
                var reminder = await _reminderService.GetReminderByIdAsync(id);
                if (reminder == null)
                {
                    _logger.LogWarning($"Reminder with ID {id} not found");
                    return NotFound(new { message = "Reminder not found" });
                }
                return Ok(reminder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving reminder with ID {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReminders()
        {
            try
            {
                var reminders = await _reminderService.GetAllRemindersAsync();
                return Ok(reminders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all reminders");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(Guid id, [FromBody] Reminder reminder)
        {
            if (id != reminder.ReminderId)
            {
                _logger.LogWarning($"Reminder ID mismatch: URL ID={id}, Body ID={reminder.ReminderId}");
                return BadRequest(new { message = "Reminder ID mismatch." });
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedReminder = await _reminderService.UpdateReminderAsync(reminder);
                if (updatedReminder == null)
                {
                    _logger.LogWarning($"Reminder with ID {id} not found for update");
                    return NotFound(new { message = "Reminder not found." });
                }
                return Ok(updatedReminder);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Null reminder data provided for update");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating reminder with ID {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(Guid id)
        {
            try
            {
                var success = await _reminderService.DeleteReminderAsync(id);
                if (!success)
                {
                    _logger.LogWarning($"Reminder with ID {id} not found for deletion");
                    return NotFound(new { message = "Reminder not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting reminder with ID {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
