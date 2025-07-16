using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories.IManagerRepositories;
using AskHire_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskHire_Backend.Repositories.ManagerRepositories
{
    public class ManagerLongListInterviewRepository : IManagerLongListInterviewRepository
    {
        private readonly AppDbContext _context;

        public ManagerLongListInterviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Vacancy> GetVacancyByNameAsync(string vacancyName)
        {
            return await _context.Vacancies
                .FirstOrDefaultAsync(v => v.VacancyName == vacancyName)
                .ConfigureAwait(false);
        }


        public async Task<Vacancy> GetVacancyByIdAsync(Guid vacancyId)
        {
            return await _context.Vacancies
                .FirstOrDefaultAsync(v => v.VacancyId == vacancyId)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Application>> GetUnscheduledApplicationsAsync(Guid vacancyId)
        {
            return await _context.Applies
                .Include(a => a.User)
                .Where(a => a.VacancyId == vacancyId)
                .Where(a => !_context.Interviews.Any(i => i.ApplicationId == a.ApplicationId))
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Application>> GetLongListApplicationsAsync(Guid vacancyId)
        {
            return await _context.Applies
                .Include(a => a.User)
                .Where(a => a.VacancyId == vacancyId)
                .Where(a => a.Status == "Longlist")
                .Where(a => !_context.Interviews.Any(i => i.ApplicationId == a.ApplicationId))
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<bool> HasExistingInterviewAsync(Guid applicationId)
        {
            return await _context.Interviews
                .AnyAsync(i => i.ApplicationId == applicationId)
                .ConfigureAwait(false);
        }

        public async Task<bool> SaveInterviewAsync(Interview interview)
        {
            try
            {
                _context.Interviews.Add(interview);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Interview>> GetScheduledInterviewsAsync(Guid vacancyId)
        {
            return await _context.Interviews
                .Include(i => i.Application)
                    .ThenInclude(a => a.User)
                .Where(i => i.Application.VacancyId == vacancyId)
                .Where(i => i.Application.Status == "Longlist" && i.Application.DashboardStatus == "Interview")
                .ToListAsync()
                .ConfigureAwait(false);
        }

        Task<bool> IManagerLongListInterviewRepository.UpdateApplicationAsync(Application application)
        {
            throw new NotImplementedException();
        }
    }
}