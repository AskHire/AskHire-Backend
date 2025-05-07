using AskHire_Backend.Data.Entities;
using AskHire_Backend.DTOs;
using AskHire_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Repositories
{
    public class CandidateDashboardRepository : ICandidateDashboardRepository
    {
        private readonly AppDbContext _context;

        public CandidateDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CandidateDashboardDto>> GetApplicationsByUserIdAsync(Guid userId)
        {
            return await _context.Applies
                .Where(a => a.UserId == userId)
                .Include(a => a.Vacancy)
                    .ThenInclude(v => v.JobRole)
                .Select(a => new CandidateDashboardDto
                {
                    ApplicationId = a.ApplicationId,
                    DashboardStatus = a.DashboardStatus,
                    VacancyName = a.Vacancy.VacancyName,
                    EndDate = a.Vacancy.EndDate,
                    JobRoleDescription = a.Vacancy.JobRole != null ? a.Vacancy.JobRole.Description : null
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteCandidateApplicationAsync(Guid applicationId)
        {
            var application = await _context.Applies.FindAsync(applicationId);
            if (application == null) return false;

            _context.Applies.Remove(application);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

