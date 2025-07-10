using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories.AdminRepositories;
using AskHire_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskHire_Backend.Data.Repositories.AdminRepositories
{
    public class AdminJobRoleRepository : IAdminJobRoleRepository
    {
        private readonly AppDbContext _context;

        public AdminJobRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobRole>> GetAllAsync() =>
            await _context.JobRoles.ToListAsync();

        public async Task<JobRole> GetByIdAsync(Guid jobId) =>
            await _context.JobRoles.FindAsync(jobId);

        public async Task AddAsync(JobRole jobRole)
        {
            await _context.JobRoles.AddAsync(jobRole);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobRole jobRole)
        {
            _context.JobRoles.Update(jobRole);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid jobId)
        {
            var jobRole = await _context.JobRoles.FindAsync(jobId);
            if (jobRole != null)
            {
                _context.JobRoles.Remove(jobRole);
                await _context.SaveChangesAsync();
            }
        }

        // ✅ New: Get total count for pagination
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.JobRoles.CountAsync();
        }

        // ✅ New: Get paginated data
        public async Task<IEnumerable<JobRole>> GetPaginatedAsync(int skip, int take)
        {
            return await _context.JobRoles
                .OrderBy(j => j.JobTitle) // Optional: order by something
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
