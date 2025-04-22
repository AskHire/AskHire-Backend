using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Get All Admin Users
        public async Task<List<User>> GetAdminsAsync()
        {
            return await _context.Users.Where(u => u.Role == "Admin").ToListAsync();
        }

        // Delete an Admin by ID
        public async Task<bool> DeleteAdminAsync(Guid UserId)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null || user.Role != "Admin")
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get All Manager Users
        public async Task<List<User>> GetManagerAsync()
        {
            return await _context.Users.Where(u => u.Role == "Manager").ToListAsync();
        }

        // Delete an Manager by ID
        public async Task<bool> DeleteManagerAsync(Guid UserId)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null || user.Role != "Manager")
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get All Candidate Users
        public async Task<List<User>> GetCandidateAsync()
        {
            return await _context.Users.Where(u => u.Role == "Candidate").ToListAsync();
        }

        // Delete an Candidate by ID
        public async Task<bool> DeleteCandidateAsync(Guid UserId)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null || user.Role != "Candidate")
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
