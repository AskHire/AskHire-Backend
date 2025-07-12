using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Repositories.AdminRepositories;
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


    }
}
