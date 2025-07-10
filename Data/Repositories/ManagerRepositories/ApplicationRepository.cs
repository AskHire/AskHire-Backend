using AskHire_Backend.Data;
using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Models.DTOs.ManagerDTOs;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace AskHire_Backend.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;

        public ApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InterviewStatusSummaryDto> GetInterviewStatusSummaryAsync()
        {
            var result = new InterviewStatusSummaryDto();

            var sql = @"
                SELECT DashboardStatus, COUNT(*) AS Count
                FROM Applies
                WHERE DashboardStatus IN ('Interview', 'Pre-Screening')
                GROUP BY DashboardStatus;
            ";

            using (DbConnection connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string status = reader.GetString(0);
                            int count = reader.GetInt32(1);

                            if (status == "Interview")
                                result.ScheduledCount = count;
                            else if (status == "Pre-Screening")
                                result.YetToScheduleCount = count;
                        }
                    }
                }
            }

            // Temporary hardcoded value — replace with actual logic if needed
            result.CompletedCount = 15;

            return result;
        }
    }
}
