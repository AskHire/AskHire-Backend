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
            if (jobRole == null) throw new ArgumentNullException(nameof(jobRole));
            return await _jobRoleRepository.CreateJobRoleAsync(jobRole);
        }

        public async Task<JobRole?> GetJobRoleByIdAsync(Guid id) =>
            await _jobRoleRepository.GetJobRoleByIdAsync(id);

        public async Task<IEnumerable<JobRole>> GetAllJobRolesAsync() =>
            await _jobRoleRepository.GetAllJobRolesAsync();

        public async Task<bool> DeleteJobRoleAsync(Guid id) =>
            await _jobRoleRepository.DeleteJobRoleAsync(id);

        public async Task<JobRole?> UpdateJobRoleAsync(JobRole jobRole)
        {
            if (jobRole == null) throw new ArgumentNullException(nameof(jobRole));
            return await _jobRoleRepository.UpdateJobRoleAsync(jobRole);
        }

        public async Task<int> GetTotalJobsAsync() =>
            await _jobRoleRepository.GetTotalJobsAsync();
    }
}
