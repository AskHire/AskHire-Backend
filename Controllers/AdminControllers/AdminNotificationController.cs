using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;

[Route("api/adminnotification")]
[ApiController]
public class AdminNotificationsController : ControllerBase
{
    private readonly IAdminNotificationService _service;

    public AdminNotificationsController(IAdminNotificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await _service.GetAllAsync();
        return Ok(notifications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var notification = await _service.GetByIdAsync(id);
        if (notification == null) return NotFound();
        return Ok(notification);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Notification notification)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(notification);
        return CreatedAtAction(nameof(GetById), new { id = created.NotificationId }, created);
    }

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
