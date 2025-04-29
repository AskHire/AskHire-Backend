using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Repositories
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

    }
}
