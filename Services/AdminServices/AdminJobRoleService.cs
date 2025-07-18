using AskHire_Backend.Interfaces.Repositories.AdminRepositories;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.AdminServices
{
    public class AdminJobRoleService : IAdminJobRoleService
    {
        private readonly IAdminJobRoleRepository _repository;

        public AdminJobRoleService(IAdminJobRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<JobRole>> GetAllAsync() =>
            await _repository.GetAllAsync();

        public async Task<JobRole> GetByIdAsync(Guid jobId) =>
            await _repository.GetByIdAsync(jobId);

        public async Task<JobRole> CreateAsync(JobRole jobRole)
        {
            await _repository.AddAsync(jobRole);
            return jobRole;
        }

        public async Task<JobRole> UpdateAsync(Guid jobId, JobRole jobRole)
        {
            var existing = await _repository.GetByIdAsync(jobId);
            if (existing == null) return null;

            existing.JobTitle = jobRole.JobTitle;
            existing.Description = jobRole.Description;
            existing.WorkType = jobRole.WorkType;
            existing.WorkLocation = jobRole.WorkLocation;

            await _repository.UpdateAsync(existing);
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid jobId)
        {
            var existing = await _repository.GetByIdAsync(jobId);
            if (existing == null) return false;

            await _repository.DeleteAsync(jobId);
            return true;
        }

        // ✅ NEW Pagination, Search, Sort method
        public async Task<PaginatedResult<JobRole>> GetPaginatedAsync(PaginationQuery query)
        {
            return await _repository.GetPaginatedAsync(query);
        }
    }
}