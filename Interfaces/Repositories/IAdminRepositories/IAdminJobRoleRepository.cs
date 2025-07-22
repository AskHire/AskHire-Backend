using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Repositories.AdminRepositories
{
    public interface IAdminJobRoleRepository
    {
        Task<IEnumerable<JobRole>> GetAllAsync();
        Task<JobRole> GetByIdAsync(Guid jobId);
        Task AddAsync(JobRole jobRole);
        Task UpdateAsync(JobRole jobRole);
        Task DeleteAsync(Guid jobId);

        // ✅ Supports pagination, search, and sorting
        Task<PaginatedResult<JobRole>> GetPaginatedAsync(PaginationQuery query);

        Task<bool> ExistsAsync(JobRole jobRole);
    }
}
