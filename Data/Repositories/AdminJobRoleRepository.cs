using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Repositories
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
    }
}
