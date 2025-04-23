using AskHire_Backend.Models.Entities;
using AskHire_Backend.Data; // Assuming you have a DbContext
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AskHire_Backend.Data.Entities;

namespace AskHire_Backend.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            if (notification.NotificationId == Guid.Empty)
            {
                notification.NotificationId = Guid.NewGuid();
            }

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task<Notification> GetNotificationByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }
    }
}