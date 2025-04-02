using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Services
{
    public interface IJobRoleService
    {
        Task<JobRole> CreateJobRoleAsync(JobRole jobRole);
        Task<JobRole?> GetJobRoleByIdAsync(Guid id);  // Nullable return type
        Task<IEnumerable<JobRole>> GetAllJobRolesAsync();
        Task<bool> DeleteJobRoleAsync(Guid id);
        Task<JobRole?> UpdateJobRoleAsync(JobRole jobRole);  // Nullable return type
    }
}
