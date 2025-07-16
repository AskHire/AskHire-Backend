using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<CandidateJobPagedResultDto<CandidateVacancyDto>> GetJobWiseVacanciesAsync(
            int pageNumber, int pageSize, string search, string sortOrder, bool isDemanded, bool isLatest)
        {
            var query = _context.Vacancies
                .Include(v => v.JobRole)
                .AsQueryable()
                // ❗ KEY FILTER: This line excludes vacancies where the end date has passed.
                // This is why you only see 18 active vacancies out of 20 total.
                .Where(v => v.EndDate >= DateTime.UtcNow);

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

            if (isLatest)
            {
                query = query.OrderByDescending(v => v.StartDate);
            }
            else if (isDemanded)
            {
                //query = query.OrderByDescending(v => v.Applies.Count());
            }
            else
            {
                sortOrder = sortOrder?.ToLower();
                query = sortOrder switch
                {
                    "a-z" => query.OrderBy(v => v.VacancyName),
                    "z-a" => query.OrderByDescending(v => v.VacancyName),
                    _ => query.OrderByDescending(v => v.StartDate)
                };
            }

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
                .Where(a => a.Vacancy.EndDate >= DateTime.UtcNow)
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
                .Where(v => v.EndDate >= DateTime.UtcNow)
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

        // MODIFIED METHOD SIGNATURE - userId is now nullable
        public async Task<(string status, CandidateJobShowDto? vacancy)> GetVacancyByIdAsync(Guid vacancyId, Guid? userId)
        {
            // Only check if already applied if a userId is provided
            if (userId.HasValue)
            {
                var hasApplied = await _context.Applies
                    .AnyAsync(a => a.VacancyId == vacancyId && a.UserId == userId.Value);

                if (hasApplied)
                {
                    return ("ALREADY_APPLIED", null);
                }
            }

            var vacancy = await _context.Vacancies
                .Where(v => v.VacancyId == vacancyId && v.EndDate >= DateTime.UtcNow)
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

            if (vacancy == null)
            {
                return ("NOT_FOUND", null);
            }

            return ("FOUND", vacancy);
        }
    }
}