using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories.AdminRepositories;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
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

        public async Task<JobRole?> GetByIdAsync(Guid jobId) =>
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

        // ✅ Pagination with search + default sorting (newest first)
        public async Task<PaginatedResult<JobRole>> GetPaginatedAsync(PaginationQuery query)
        {
            var dataQuery = _context.JobRoles.AsQueryable();

            // 🔍 Search by JobTitle
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                string term = query.SearchTerm.Trim();
                dataQuery = dataQuery.Where(j => j.JobTitle.Contains(term));
            }

            // 🔃 Sorting logic
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.Trim().ToLower())
                {
                    case "jobtitle":
                        dataQuery = query.IsDescending
                            ? dataQuery.OrderByDescending(j => j.JobTitle)
                            : dataQuery.OrderBy(j => j.JobTitle);
                        break;

                    case "createddate":
                        dataQuery = query.IsDescending
                            ? dataQuery.OrderByDescending(j => j.CreatedAt)
                            : dataQuery.OrderBy(j => j.CreatedAt);
                        break;

                    default:
                        // Default: newest jobs first
                        dataQuery = dataQuery.OrderByDescending(j => j.CreatedAt);
                        break;
                }
            }
            else
            {
                // Default sorting: newest first
                dataQuery = dataQuery.OrderByDescending(j => j.CreatedAt);
            }

            // 📄 Pagination
            var totalCount = await dataQuery.CountAsync();
            var items = await dataQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedResult<JobRole>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
                Data = items
            };
        }

        public async Task<bool> ExistsAsync(JobRole jobRole)
        {
            var title = jobRole.JobTitle.Trim().ToLower();
            var desc = jobRole.Description.Trim();

            return await _context.JobRoles.AnyAsync(j =>
                j.JobTitle.ToLower() == title &&
                j.Description.Trim() == desc &&
                j.WorkLocation == jobRole.WorkLocation &&
                j.WorkType == jobRole.WorkType
            );
        }
    }
}
