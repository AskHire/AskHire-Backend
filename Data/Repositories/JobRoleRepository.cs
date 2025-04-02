using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Data.Entities;

namespace AskHire_Backend.Data.Repositories
{
    public class JobRoleRepository : IJobRoleRepository
    {
        private readonly AppDbContext _context;

        public JobRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JobRole> CreateJobRoleAsync(JobRole jobRole)
        {
            _context.JobRoles.Add(jobRole);
            await _context.SaveChangesAsync();
            return jobRole;
        }

        public async Task<JobRole?> GetJobRoleByIdAsync(Guid id)
        {
            return await _context.JobRoles.FindAsync(id);
        }

        public async Task<IEnumerable<JobRole>> GetAllJobRolesAsync()
        {
            return await _context.JobRoles.ToListAsync();
        }

        public async Task<bool> DeleteJobRoleAsync(Guid id)
        {
            var jobRole = await _context.JobRoles.FindAsync(id);
            if (jobRole == null)
            {
                return false;
            }

            _context.JobRoles.Remove(jobRole);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<JobRole?> UpdateJobRoleAsync(JobRole jobRole)
        {
            var existingJobRole = await _context.JobRoles.FindAsync(jobRole.JobId);
            if (existingJobRole == null)
            {
                return null;
            }

            _context.Entry(existingJobRole).CurrentValues.SetValues(jobRole);
            await _context.SaveChangesAsync();
            return existingJobRole;
        }
    }
}
