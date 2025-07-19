using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Interfaces.Services
{
    public interface INotificationShowServices
    {
        Task<IEnumerable<NotificationShowDto>> GetAllNotificationsAsync();
        Task<IEnumerable<NotificationShowDto>> GetAdminNotificationsAsync();
    }
}