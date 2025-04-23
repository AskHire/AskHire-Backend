using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Services;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // POST: api/Notification
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] Notification notification)
        {
            if (notification == null)
            {
                return BadRequest("Notification is null.");
            }

            var createdNotification = await _notificationService.CreateNotificationAsync(notification).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetNotificationById),
                new { id = createdNotification.NotificationId },
                createdNotification);
        }

        // GET: api/Notification
        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync().ConfigureAwait(false);
            return Ok(notifications);
        }

        // GET: api/Notification/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(Guid id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id).ConfigureAwait(false);

            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }
    }
}
