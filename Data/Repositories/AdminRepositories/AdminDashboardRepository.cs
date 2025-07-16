using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Repositories.AdminRepositories;
using AskHire_Backend.Models.DTOs.AdminDTOs;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Data.Repositories.AdminRepositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly AppDbContext _context;

        public AdminDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetTotalManagersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Manager")
                .CountAsync();
        }

        public async Task<int> GetTotalCandidatesAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Candidate")
                .CountAsync();
        }

        public async Task<int> GetTotalJobsAsync()
        {
            return await _context.JobRoles.CountAsync();
        }

        public async Task<List<int>> GetMonthlySignupsAsync()
        {
            var currentYear = DateTime.UtcNow.Year;

            var monthlyData = await _context.Users
                .Where(u => u.SignUpDate.HasValue && u.SignUpDate.Value.Year == currentYear)
                .GroupBy(u => u.SignUpDate.Value.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = new int[12]; // Jan-Dec

            foreach (var item in monthlyData)
            {
                result[item.Month - 1] = item.Count;
            }

            return result.ToList();
        }

        public async Task<Dictionary<string, int>> GetUsersByAgeGroupAsync()
        {
            var users = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.DOB))
                .ToListAsync();

            var grouped = users
                .Select(u =>
                {
                    var age = 0;
                    if (DateTime.TryParse(u.DOB, out DateTime dob))
                    {
                        age = DateTime.Today.Year - dob.Year;
                        if (dob > DateTime.Today.AddYears(-age)) age--;
                    }

                    return age switch
                    {
                        < 20 => "< 20",
                        <= 30 => "21–30",
                        <= 40 => "31–40",
                        _ => "41+"
                    };
                })
                .GroupBy(group => group)
                .ToDictionary(g => g.Key, g => g.Count());

            return grouped;
        }

        public async Task<PaginatedResult<VacancyDashboardDto>> GetPagedVacancyTrackingTableAsync(PaginationQuery query)
        {
            var today = DateTime.UtcNow.Date;
            var vacanciesQuery = _context.Vacancies.Include(v => v.JobRole).AsQueryable();

            // Filter by Status
            if (!string.IsNullOrEmpty(query.StatusFilter))
            {
                if (query.StatusFilter == "Open")
                    vacanciesQuery = vacanciesQuery.Where(v => today >= v.StartDate && today <= v.EndDate);
                else if (query.StatusFilter == "Expired")
                    vacanciesQuery = vacanciesQuery.Where(v => today < v.StartDate || today > v.EndDate);
            }

            // Filter by Job Role Search Term
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                vacanciesQuery = vacanciesQuery.Where(v => v.JobRole != null && v.JobRole.JobTitle.Contains(query.SearchTerm));
            }

            // Sorting - support JobRole or ApplicationsCount
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                if (query.SortBy.Equals("JobRole", StringComparison.OrdinalIgnoreCase))
                {
                    vacanciesQuery = query.IsDescending
                        ? vacanciesQuery.OrderByDescending(v => v.JobRole.JobTitle)
                        : vacanciesQuery.OrderBy(v => v.JobRole.JobTitle);
                }
                // ApplicationsCount sorting will be done after projection (see below)
            }

            // Fetch list first (you need applications count which requires additional queries)
            var vacanciesList = await vacanciesQuery.ToListAsync();

            // Map to DTO and get application counts
            var vacancyDtos = new List<VacancyDashboardDto>();
            foreach (var vacancy in vacanciesList)
            {
                int applicationCount = await _context.Applies.CountAsync(a => a.VacancyId == vacancy.VacancyId);

                vacancyDtos.Add(new VacancyDashboardDto
                {
                    VacancyName = vacancy.VacancyName,
                    JobRole = vacancy.JobRole?.JobTitle ?? "N/A",
                    StartDate = vacancy.StartDate,
                    EndDate = vacancy.EndDate,
                    Status = (today >= vacancy.StartDate && today <= vacancy.EndDate) ? "Open" : "Expired",
                    ApplicationsCount = applicationCount
                });
            }

            // Sort by ApplicationsCount if requested
            if (query.SortBy != null && query.SortBy.Equals("ApplicationsCount", StringComparison.OrdinalIgnoreCase))
            {
                vacancyDtos = query.IsDescending
                    ? vacancyDtos.OrderByDescending(v => v.ApplicationsCount).ToList()
                    : vacancyDtos.OrderBy(v => v.ApplicationsCount).ToList();
            }

            // Pagination
            var totalCount = vacancyDtos.Count;
            var pagedData = vacancyDtos
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new PaginatedResult<VacancyDashboardDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
                Data = pagedData
            };
        }





    }
}
