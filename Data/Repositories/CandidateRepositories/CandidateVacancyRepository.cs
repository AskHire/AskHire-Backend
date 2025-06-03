using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
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

        public async Task<CandidateJobPagedResultDto<CandidateVacancyDto>> GetJobWiseVacanciesAsync(int pageNumber, int pageSize, string search, string sortOrder)
        {
            var query = _context.Vacancies
                .Include(v => v.JobRole)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(v =>
                    v.VacancyName.ToLower().Contains(search) ||
                    (v.JobRole != null && (
                        v.JobRole.Description.ToLower().Contains(search) ||
                        v.JobRole.WorkLocation.ToLower().Contains(search) ||
                        v.JobRole.WorkType.ToLower().Contains(search)
                    ))
                );
            }

            // ✅ Apply sorting
            sortOrder = sortOrder?.ToLower();
            query = sortOrder switch
            {
                "a-z" => query.OrderBy(v => v.VacancyName),
                "z-a" => query.OrderByDescending(v => v.VacancyName),
                _ => query.OrderBy(v => v.VacancyName) // Default order
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            return new CandidateJobPagedResultDto<CandidateVacancyDto>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }






        public async Task<IEnumerable<CandidateVacancyDto>> GetMostAppliedVacanciesAsync()
        {
            var topVacancyIds = await _context.Applies
                .GroupBy(a => a.VacancyId)
                .OrderByDescending(g => g.Count())
                .Take(6)
                .Select(g => g.Key)
                .ToListAsync();

            return await _context.Vacancies
                .Where(v => topVacancyIds.Contains(v.VacancyId))
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


        public async Task<IEnumerable<CandidateVacancyDto>> GetLatestVacanciesAsync()
        {
            return await _context.Vacancies
                .OrderByDescending(v => v.StartDate)
                .Take(6)
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

        public async Task<CandidateJobShowDto?> GetVacancyByIdAsync(Guid vacancyId)
        {
            return await _context.Vacancies
                .Where(v => v.VacancyId == vacancyId)
                .Include(v => v.JobRole)
                .Select(v => new CandidateJobShowDto
                {
                    VacancyName = v.VacancyName,
                    Instructions = v.Instructions,
                    Experience = v.Experience,
                    Education = v.Education,
                    NonTechnicalSkills = v.NonTechnicalSkills,
                    StartDate = v.StartDate,
                    RequiredSkills = v.RequiredSkills,
                    EndDate = v.EndDate,
                    Description = v.JobRole != null ? v.JobRole.Description : "N/A",
                    WorkType = v.JobRole != null ? v.JobRole.WorkType : "N/A",
                    WorkLocation = v.JobRole != null ? v.JobRole.WorkLocation : "N/A"
                })
                .FirstOrDefaultAsync();
        }


    }
}
