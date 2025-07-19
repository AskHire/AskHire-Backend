using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Interfaces.Repositories
{
    public interface INotificationShowRepository
    {
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();
        Task<IEnumerable<Notification>> GetNotificationsByStatusAsync(string status);
    }
}
