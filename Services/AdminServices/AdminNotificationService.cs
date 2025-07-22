using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Models.Entities;

public class AdminNotificationService : IAdminNotificationService
{
    private readonly IAdminNotificationRepository _repository;

    public AdminNotificationService(IAdminNotificationRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Notification>> GetAllAsync() =>
        _repository.GetAllAsync();

    public Task<Notification?> GetByIdAsync(Guid id) =>
        _repository.GetByIdAsync(id);

    public async Task<Notification> CreateAsync(Notification notification)
    {
        // Defensive fallback
        if (string.IsNullOrWhiteSpace(notification.Status))
            notification.Status = "Admin";

        if (notification.Time == default)
            notification.Time = DateTime.Now;

        return await _repository.CreateAsync(notification);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var exists = await _repository.GetByIdAsync(id);
        if (exists == null) return false;
        return await _repository.DeleteAsync(id);
    }

    public Task<PaginatedResult<Notification>> GetPagedAsync(PaginationQuery query) =>
        _repository.GetPagedAsync(query);
}
