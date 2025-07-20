using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Data.Entities; // Add this using statement
using Microsoft.EntityFrameworkCore; // Add this using statement

namespace AskHire_Backend.Data.Repositories
{
    public class NotificationShowRepository : INotificationShowRepository
    {
        private readonly AppDbContext _context; // Use your DbContext

        public NotificationShowRepository(AppDbContext context) // Inject DbContext
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            // Fetch all notifications from the database
            return await _context.Notifications.ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByStatusAsync(string status)
        {
            // Fetch notifications by status from the database
            return await _context.Notifications.Where(n => n.Status == status).ToListAsync();
        }
    }
}