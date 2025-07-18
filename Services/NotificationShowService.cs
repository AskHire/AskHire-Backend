using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Services
{
    public class NotificationShowService : INotificationShowServices
    {
        private readonly INotificationShowRepository _notificationShowRepository;

        public NotificationShowService(INotificationShowRepository notificationShowRepository)
        {
            _notificationShowRepository = notificationShowRepository;
        }

        public async Task<IEnumerable<NotificationShowDto>> GetAllNotificationsAsync()
        {
            var notifications = await _notificationShowRepository.GetAllNotificationsAsync();
            return notifications.Select(n => new NotificationShowDto
            {
                NotificationId = n.NotificationId,
                Message = n.Message,
                Time = n.Time,
                Type = n.Type,
                Subject = n.Subject,
                Status = n.Status
            }).ToList();
        }

        public async Task<IEnumerable<NotificationShowDto>> GetAdminNotificationsAsync()
        {
            var notifications = await _notificationShowRepository.GetNotificationsByStatusAsync("Admin");
            return notifications.Select(n => new NotificationShowDto
            {
                NotificationId = n.NotificationId,
                Message = n.Message,
                Time = n.Time,
                Type = n.Type,
                Subject = n.Subject,
                Status = n.Status
            }).ToList();
        }
    }
}