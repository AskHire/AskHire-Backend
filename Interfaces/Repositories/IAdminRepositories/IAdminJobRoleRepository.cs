using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Interfaces.Repositories.AdminRepositories
{
    public interface IAdminJobRoleRepository
    {
        Task<IEnumerable<JobRole>> GetAllAsync();
        Task<JobRole> GetByIdAsync(Guid jobId);
        Task AddAsync(JobRole jobRole);
        Task UpdateAsync(JobRole jobRole);
        Task DeleteAsync(Guid jobId);
    }
}
