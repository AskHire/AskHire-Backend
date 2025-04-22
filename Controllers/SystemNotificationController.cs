using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _context.Notifications
                .OrderByDescending(n => n.Time)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Notification notification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = notification.NotificationId }, notification);
        }
    }
}
