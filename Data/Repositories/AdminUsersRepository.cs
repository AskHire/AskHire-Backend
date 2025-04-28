using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace AskHire_Backend.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {

        private readonly AppDbContext _context;
        private readonly string _connectionString;

        public AdminUserRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _connectionString = configuration.GetConnectionString("DatabaseString") 
                ?? throw new InvalidOperationException("Connection string 'DatabaseString' not found.");
        }


        // Removed duplicate _context declaration and constructor

        public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task<User> GetByIdAsync(Guid id) => await _context.Users.FindAsync(id);

        public async Task<User> AddAsync(User user)
        {
            user.Id = Guid.NewGuid(); // Changed from user.UserId
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<int> GetTotalUsersAsync() 
        {
            {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("SELECT COUNT(*) FROM AspNetUsers WHERE role = 'candidate' ", connection);
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(string role) =>
            await _context.Users.Where(u => u.Role == role).ToListAsync();
    }
}
