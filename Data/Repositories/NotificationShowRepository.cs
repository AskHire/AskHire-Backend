using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Data.Repositories
{
    // This is a simplified in-memory repository for demonstration.
    // In a real application, you would interact with a database context (e.g., Entity Framework Core).
    public class NotificationShowRepository : INotificationShowRepository
    {
        private readonly List<Notification> _notifications = new List<Notification>
        {
            new Notification { NotificationId = Guid.NewGuid(), Message = "Welcome Admin!", Time = DateTime.UtcNow, Type = "System", Subject = "Welcome", Status = "Admin" },
            new Notification { NotificationId = Guid.NewGuid(), Message = "New user registered.", Time = DateTime.UtcNow.AddMinutes(-10), Type = "Info", Subject = "User Registration", Status = "Pending" },
            new Notification { NotificationId = Guid.NewGuid(), Message = "Critical error detected.", Time = DateTime.UtcNow.AddHours(-1), Type = "Error", Subject = "System Alert", Status = "Admin" },
            new Notification { NotificationId = Guid.NewGuid(), Message = "Password updated successfully.", Time = DateTime.UtcNow.AddDays(-1), Type = "Security", Subject = "Account Update", Status = "User" }
        };

        public Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return Task.FromResult<IEnumerable<Notification>>(_notifications);
        }

        public Task<IEnumerable<Notification>> GetNotificationsByStatusAsync(string status)
        {
            return Task.FromResult<IEnumerable<Notification>>(_notifications.Where(n => n.Status == status).ToList());
        }
    }
}
