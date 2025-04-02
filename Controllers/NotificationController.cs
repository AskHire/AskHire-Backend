using Microsoft.AspNetCore.Mvc;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Data.Entities; // Update the namespace to match your DbContext
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context; // Use AppDbContext instead of ApplicationDbContext

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Notification
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] Notification notification)
        {
            if (notification == null)
            {
                return BadRequest("Notification is null.");
            }

            // Generate a new GUID for the notification
            notification.NotificationId = Guid.NewGuid();

            // Add the notification to the database
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Return the created notification with a 201 status code
            return CreatedAtAction(nameof(GetNotificationById), new { id = notification.NotificationId }, notification);
        }

        // GET: api/Notification
        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            // Return the list of notifications from the database
            var notifications = await _context.Notifications.ToListAsync();
            return Ok(notifications);
        }

        // GET: api/Notification/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(Guid id)
        {
            // Find the notification by ID
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound(); // Return 404 if the notification is not found
            }

            // Return the notification
            return Ok(notification);
        }
    }
}