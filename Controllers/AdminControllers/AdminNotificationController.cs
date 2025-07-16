using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AskHire_Backend.Controllers.AdminNotificationControllers
{
    [Route("api/adminnotification")]
    [ApiController]
    public class AdminNotificationsController : ControllerBase
    {
        private readonly IAdminNotificationService _service;

        public AdminNotificationsController(IAdminNotificationService service)
        {
            _service = service;
        }

        // GET api/adminnotification?Page=1&PageSize=10&SortBy=Time&IsDescending=true&SearchTerm=foo
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery query)
        {
            var pagedResult = await _service.GetPagedAsync(query);
            return Ok(pagedResult);
        }

        // GET api/adminnotification/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var notification = await _service.GetByIdAsync(id);
            if (notification == null) return NotFound();
            return Ok(notification);
        }

        // POST api/adminnotification
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Notification notification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(notification);
            return CreatedAtAction(nameof(GetById), new { id = created.NotificationId }, created);
        }

        // DELETE api/adminnotification/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var notification = await _service.GetByIdAsync(id);
            if (notification == null) return NotFound();

            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return BadRequest("Failed to delete notification.");

            return NoContent();
        }
    }
}
