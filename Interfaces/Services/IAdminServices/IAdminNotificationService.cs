using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Models.Entities;

public interface IAdminNotificationService
{
    Task<List<Notification>> GetAllAsync();
    Task<Notification?> GetByIdAsync(Guid id);
    Task<Notification> CreateAsync(Notification notification);
    Task<bool> DeleteAsync(Guid id);
    Task<PaginatedResult<Notification>> GetPagedAsync(PaginationQuery query);
}
