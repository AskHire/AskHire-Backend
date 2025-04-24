using AskHire_Backend.Models.Entities;

public class   AdminNotificationService : IAdminNotificationService
{
    private readonly IAdminNotificationRepository _repository;

    public AdminNotificationService(IAdminNotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Notification>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        return await _repository.CreateAsync(notification);
    }
}
