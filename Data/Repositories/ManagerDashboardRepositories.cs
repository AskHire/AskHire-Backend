using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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

        public async Task<Dictionary<string, int>> GetWeeklyInterviewCountAsync()
        {
            var today = DateTime.Today;
            var monday = today.AddDays(-(int)(today.DayOfWeek == DayOfWeek.Sunday ? 6 : today.DayOfWeek - DayOfWeek.Monday));
            var sunday = monday.AddDays(6);

            var result = new Dictionary<string, int>();

            for (int i = 0; i < 7; i++)
            {
                var date = monday.AddDays(i).Date;
                result[date.ToString("yyyy-MM-dd")] = 0;
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"
                SELECT CAST([date] AS DATE) AS InterviewDate, COUNT(*) AS InterviewCount
                FROM Interviews
                WHERE CAST([date] AS DATE) BETWEEN @Monday AND @Sunday
                GROUP BY CAST([date] AS DATE)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Monday", monday);
            command.Parameters.AddWithValue("@Sunday", sunday);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var date = Convert.ToDateTime(reader["InterviewDate"]).ToString("yyyy-MM-dd");
                var count = Convert.ToInt32(reader["InterviewCount"]);
                result[date] = count;
            }

            return result;
        }
    }
}
