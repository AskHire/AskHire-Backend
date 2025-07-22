using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public interface IAdminJobRoleService
    {
        Task<IEnumerable<JobRole>> GetAllAsync();
        Task<JobRole> GetByIdAsync(Guid jobId);
        Task<JobRole> CreateAsync(JobRole jobRole);
        Task<JobRole> UpdateAsync(Guid jobId, JobRole jobRole);
        Task<bool> DeleteAsync(Guid jobId);

        // ✅ Updated method  
        Task<PaginatedResult<JobRole>> GetPaginatedAsync(PaginationQuery query);
    }
}