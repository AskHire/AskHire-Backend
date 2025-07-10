using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Repositories
{
    public class ManagerDashboardRepository : IManagerDashboardRepository
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;

        public ManagerDashboardRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _connectionString = configuration.GetConnectionString("DatabaseString")
                ?? throw new InvalidOperationException("Connection string 'DatabaseString' not found.");
        }

        public async Task<int> GetTotalUsersAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT COUNT(*) FROM AspNetUsers WHERE role = 'candidate'";
            using var command = new SqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<int> GetTotalJobsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT COUNT(*) FROM Vacancies";
            using var command = new SqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<int> GetTotalInterviewsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT COUNT(*) FROM Interviews WHERE CAST([date] AS DATE) = CAST(GETDATE() AS DATE)";
            using var command = new SqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}