using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class AdminNotificationRepository : IAdminNotificationRepository
{
    private readonly AppDbContext _context;

    public AdminNotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetAllAsync()
    {
        return await _context.Notifications.OrderByDescending(n => n.Time).ToListAsync();
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null)
        {
            return false; // Notification not found
        }
        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
        return true;
    }
}
