using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Services.IManagerServices
{
    public interface IManagerNotificationService
    {
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task<Notification> GetNotificationByIdAsync(Guid id);
    }
}