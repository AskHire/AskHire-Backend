using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public class JobRoleService : Interfaces.Services.IJobRoleService
    {
        private readonly IJobRoleRepository _jobRoleRepository;

        public JobRoleService(IJobRoleRepository jobRoleRepository)
        {
            _jobRoleRepository = jobRoleRepository ?? throw new ArgumentNullException(nameof(jobRoleRepository));
        }

        public async Task<JobRole> CreateJobRoleAsync(JobRole jobRole)
        {
            if (jobRole == null)
            {
                throw new ArgumentNullException(nameof(jobRole), "Job role cannot be null.");
            }

            return await _jobRoleRepository.CreateJobRoleAsync(jobRole);
        }

        public async Task<JobRole?> GetJobRoleByIdAsync(Guid id)
        {
            return await _jobRoleRepository.GetJobRoleByIdAsync(id); // This can return null, which matches the nullable return type
        }

        public async Task<IEnumerable<JobRole>> GetAllJobRolesAsync()
        {
            return await _jobRoleRepository.GetAllJobRolesAsync();
        }

        public async Task<bool> DeleteJobRoleAsync(Guid id)
        {
            return await _jobRoleRepository.DeleteJobRoleAsync(id);
        }

        public async Task<JobRole?> UpdateJobRoleAsync(JobRole jobRole)
        {
            if (jobRole == null)
            {
                throw new ArgumentNullException(nameof(jobRole), "Job role cannot be null.");
            }

            return await _jobRoleRepository.UpdateJobRoleAsync(jobRole); // This can also return null, which matches the nullable return type
        }
    }
}
