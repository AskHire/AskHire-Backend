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
                SELECT
                    -- Count of Scheduled Interviews (interview date > today)
                    (SELECT COUNT(*) 
                     FROM Interviews 
                     WHERE Date > CAST(GETDATE() AS DATE)) AS ScheduledInterviews,

                    -- Count of Completed Interviews (interview date < today)
                    (SELECT COUNT(*) 
                     FROM Interviews 
                     WHERE Date < CAST(GETDATE() AS DATE)) AS CompletedInterviews,

                    -- Count of Yet to Schedule Interviews (DashboardStatus = 'Pre-Screening')
                    (SELECT COUNT(*) 
                     FROM Applies 
                     WHERE DashboardStatus = 'Pre-Screening') AS YetToScheduleInterviews;
            ";

            using (DbConnection connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result.ScheduledCount = reader.GetInt32(0);
                            result.CompletedCount = reader.GetInt32(1);
                            result.YetToScheduleCount = reader.GetInt32(2);
                        }
                    }
                }
            }

            return result;
        }
    }
}
