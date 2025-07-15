using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
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

    public async Task<bool> DeleteAsync(Guid id)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification == null)
        {
            return false; // Notification not found
        }
        // Assuming the repository has a method to delete by id
        return await _repository.DeleteAsync(id);
    }

    public async Task<PaginatedResult<Notification>> GetPagedAsync(PaginationQuery query)
    {
        return await _repository.GetPagedAsync(query);
    }

}
