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
            int pageNumber, int pageSize, string search, string sortOrder, bool isDemanded, bool isLatest,
            string workLocation, string workType) // New parameters
        {
            var query = _context.Vacancies
                .Include(v => v.JobRole)
                .AsQueryable()
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

            // Apply Work Location Filter
            if (!string.IsNullOrWhiteSpace(workLocation) && workLocation.ToLower() != "all")
            {
                query = query.Where(v => v.JobRole != null && v.JobRole.WorkLocation.ToLower() == workLocation.ToLower());
            }

            // Apply Work Type Filter
            if (!string.IsNullOrWhiteSpace(workType) && workType.ToLower() != "all")
            {
                query = query.Where(v => v.JobRole != null && v.JobRole.WorkType.ToLower() == workType.ToLower());
            }

            // The 'isDemanded' and 'isLatest' filters should be mutually exclusive with other sorting,
            // or applied in a specific order if combined.
            // In the current setup, if isLatest or isDemanded is true, they override sortOrder.
            if (isLatest)
            {
                query = query.OrderByDescending(v => v.StartDate);
            }
            else if (isDemanded)
            {
                // To get demanded jobs, you need to count applications.
                // This requires a GroupJoin or explicit joins if Vacancy does not have an Applies navigation property.
                query = query
                    .GroupJoin(
                        _context.Applies,
                        vacancy => vacancy.VacancyId,
                        apply => apply.VacancyId,
                        (vacancy, applies) => new { Vacancy = vacancy, ApplyCount = applies.Count() }
                    )
                    .OrderByDescending(x => x.ApplyCount)
                    .Select(x => x.Vacancy); // Select back the Vacancy entity
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
            // The approach here is largely correct.
            // It explicitly joins and groups by VacancyId from the Applies table.
            var topVacancyIds = await _context.Applies
                .Where(a => a.Vacancy.EndDate >= DateTime.UtcNow) // Ensure the vacancy is still active
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

        public async Task<(string status, CandidateJobShowDto? vacancy)> GetVacancyByIdAsync(Guid vacancyId, Guid? userId)
        {
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