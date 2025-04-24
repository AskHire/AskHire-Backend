using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Repositories
{
    public interface IJobRoleRepository
    {
        Task<JobRole> CreateJobRoleAsync(JobRole jobRole);
        Task<JobRole?> GetJobRoleByIdAsync(Guid id);
        Task<IEnumerable<JobRole>> GetAllJobRolesAsync();
        Task<bool> DeleteJobRoleAsync(Guid id);
        Task<JobRole?> UpdateJobRoleAsync(JobRole jobRole);
        Task<int> GetTotalJobsAsync();
        // Removed: Task<int> GetTotalJobsAsync(JobRole);
    }
}