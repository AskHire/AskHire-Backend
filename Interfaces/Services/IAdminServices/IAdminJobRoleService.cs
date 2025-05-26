using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Services
{
    public interface IAdminJobRoleService
    {
        Task<IEnumerable<JobRole>> GetAllAsync();
        Task<JobRole> GetByIdAsync(Guid jobId);
        Task<JobRole> CreateAsync(JobRole jobRole);
        Task<JobRole> UpdateAsync(Guid jobId, JobRole jobRole);
        Task<bool> DeleteAsync(Guid jobId);
    }
}
