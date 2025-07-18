using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // Added for List
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Interfaces.Services;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationShowController : ControllerBase
    {
        private readonly INotificationShowServices _notificationShowServices;

        public NotificationShowController(INotificationShowServices notificationShowServices)
        {
            _notificationShowServices = notificationShowServices;
        }

        /// <summary>
        /// Retrieves all notifications.
        /// </summary>
        /// <returns>A list of all notifications.</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NotificationShowDto>))]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _notificationShowServices.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        /// <summary>
        /// Retrieves notifications with Status set to "Admin".
        /// </summary>
        /// <returns>A list of "Admin" notifications.</returns>
        [HttpGet("admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NotificationShowDto>))]
        public async Task<IActionResult> GetAdminNotifications()
        {
            var notifications = await _notificationShowServices.GetAdminNotificationsAsync();
            return Ok(notifications);
        }
    }
}