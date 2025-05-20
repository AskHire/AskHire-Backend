using AskHire_Backend.Models.Entities;

public interface IAdminNotificationService
{
    Task<List<Notification>> GetAllAsync();
    Task<Notification?> GetByIdAsync(Guid id);
    Task<Notification> CreateAsync(Notification notification);
}
