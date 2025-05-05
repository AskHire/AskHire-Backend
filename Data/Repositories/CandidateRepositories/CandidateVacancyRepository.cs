using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskHire_Backend.Repositories
{
    public class CandidateVacancyRepository : ICandidateVacancyRepository
    {
        private readonly AppDbContext _context;

        public CandidateVacancyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CandidateVacancyDto>> GetJobWiseVacanciesAsync()
        {
            return await _context.Vacancies
                .Include(v => v.JobRole)
                .Select(v => new CandidateVacancyDto
                {
                    VacancyId = v.VacancyId,
                    VacancyName = v.VacancyName,
                    WorkType = v.JobRole != null ? v.JobRole.WorkType : "N/A",
                    WorkLocation = v.JobRole != null ? v.JobRole.WorkLocation : "N/A",
                    Description = v.JobRole != null ? v.JobRole.Description : "N/A",
                    Instructions = v.Instructions,
                    EndDate = v.EndDate
                })
                .ToListAsync();
        }
    }
}
