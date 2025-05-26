using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskHire_Backend.Models;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories.ManagerRepositories;

namespace AskHire_Backend.Data.Repositories.ManagerRepositories
{
    public class ManagerCandidateRepository : IManagerCandidateRepository
    {
        private readonly AppDbContext _context;

        public ManagerCandidateRepository(AppDbContext context)
        {
            _context = context;
        }


        // Fetch all available vacancies
        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync()
        {
            return await _context.Vacancies.ToListAsync(); // Fetch vacancies from the database
        }

        public async Task<IEnumerable<object>> GetAllApplicationsAsync()
        {
            return await _context.Applies
                .Include(a => a.User)
                .Include(a => a.Vacancy)
                .ToListAsync();
        }

        public async Task<object> GetApplicationByIdAsync(Guid applicationId)
        {
            return await _context.Applies
                .Include(a => a.User)
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
        }

        public async Task<IEnumerable<object>> GetApplicationsByVacancyNameAsync(string vacancyName)
        {
            // Find job role by name
            var jobRole = await _context.JobRoles
                .FirstOrDefaultAsync(j => EF.Functions.Like(j.JobTitle.ToLower(), $"%{vacancyName.ToLower()}%"));

            if (jobRole == null)
            {
                return new List<object>();
            }

            // Get matching vacancies based on JobId
            var matchingVacancies = await _context.Vacancies
                .Where(v => v.JobId == jobRole.JobId)
                .Select(v => v.VacancyId)
                .ToListAsync();

            // Fetch Applications linked to those vacancies
            return await _context.Applies
                .Include(a => a.User)
                .Include(a => a.Vacancy)
                .Where(a => matchingVacancies.Contains(a.VacancyId))
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetApplicationsByStatusAsync(string status)
        {
            return await _context.Applies
                .Include(a => a.User)
                .Include(a => a.Vacancy)
                    .ThenInclude(v => v.JobRole)
                .Where(a => a.Status.ToLower() == status.ToLower())
                .ToListAsync();
        }

        public async Task<bool> VacancyExistsAsync(Guid vacancyId)
        {
            return await _context.Vacancies.AnyAsync(v => v.VacancyId == vacancyId);
        }

        public async Task<int> GetApplicationCountByStatusAsync(string status)
        {
            return await _context.Applies
                .Where(a => a.Status.ToLower() == status.ToLower())
                .CountAsync();
        }

        public async Task<int> GetApplicationCountByVacancyAndStatusAsync(Guid vacancyId, string status)
        {
            return await _context.Applies
                .Where(a => a.VacancyId == vacancyId && a.Status.ToLower() == status.ToLower())
                .CountAsync();
        }

        public async Task<string> GetCVPathAsync(Guid applicationId)
        {
            var application = await _context.Applies
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            return application?.CVFilePath;
        }
    }
}