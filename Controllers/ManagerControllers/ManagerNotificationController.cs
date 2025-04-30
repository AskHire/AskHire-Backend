using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.Entities;
using System;
using System.Threading.Tasks;
using AskHire_Backend.Interfaces.Services.IManagerServices;

namespace AskHire_Backend.Controllers.Manager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerNotificationController : ControllerBase
    {
        private readonly IManagerNotificationService _notificationService;

        public ManagerNotificationController(IManagerNotificationService notificationService)
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
