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

        // ✅ New unified pagination method with search + sort
        public async Task<PaginatedResult<JobRole>> GetPaginatedAsync(PaginationQuery query)
        {
            var dataQuery = _context.JobRoles.AsQueryable();

            // 🔍 Search by JobTitle
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                dataQuery = dataQuery.Where(j => j.JobTitle.Contains(query.SearchTerm));
            }

            // 🔃 Sort by JobTitle
            if (!string.IsNullOrWhiteSpace(query.SortBy) && query.SortBy.Equals("JobTitle", StringComparison.OrdinalIgnoreCase))
            {
                dataQuery = query.IsDescending
                    ? dataQuery.OrderByDescending(j => j.JobTitle)
                    : dataQuery.OrderBy(j => j.JobTitle);
            }
            else
            {
                // Optional: default sort by JobTitle ascending
                dataQuery = dataQuery.OrderBy(j => j.JobTitle);
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
    }
}
