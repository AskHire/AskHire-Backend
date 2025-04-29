using AskHire_Backend.Models.Entities;
using AskHire_Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using AskHire_Backend.Data.Entities;

namespace AskHire_Backend.Data.Repositories
{
    public class JobRoleRepository : IJobRoleRepository
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;

        public JobRoleRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _connectionString = configuration.GetConnectionString("DatabaseString") 
                ?? throw new InvalidOperationException("Connection string 'DatabaseString' not found.");
        }

        public async Task<JobRole> CreateJobRoleAsync(JobRole jobRole)
        {
            _context.JobRoles.Add(jobRole);
            await _context.SaveChangesAsync();
            return jobRole;
        }

        public async Task<JobRole?> GetJobRoleByIdAsync(Guid id)
        {
            return await _context.JobRoles.FindAsync(id);
        }

        public async Task<IEnumerable<JobRole>> GetAllJobRolesAsync()
        {
            return await _context.JobRoles.ToListAsync();
        }

        public async Task<bool> DeleteJobRoleAsync(Guid id)
        {
            var jobRole = await _context.JobRoles.FindAsync(id);
            if (jobRole == null) return false;

            _context.JobRoles.Remove(jobRole);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<JobRole?> UpdateJobRoleAsync(JobRole jobRole)
        {
            var existingJobRole = await _context.JobRoles.FindAsync(jobRole.JobId);
            if (existingJobRole == null) return null;

            _context.Entry(existingJobRole).CurrentValues.SetValues(jobRole);
            await _context.SaveChangesAsync();
            return existingJobRole;
        }

        // public async Task<int> GetTotalJobsAsync()
        // {
        //     using var connection = new SqlConnection(_connectionString);
        //     await connection.OpenAsync();

        //     using var command = new SqlCommand("SELECT COUNT(*) FROM JobRoles", connection);
        //     var result = await command.ExecuteScalarAsync();

        //     return result != null ? Convert.ToInt32(result) : 0;
        // }

        public async Task<int> GetTotalJobsAsync(JobRole jobRole)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT COUNT(*) FROM JobRoles WHERE JobTitle = @JobTitle", connection);
            command.Parameters.AddWithValue("@JobTitle", jobRole.JobTitle);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}
