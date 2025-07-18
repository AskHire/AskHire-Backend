using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Services
{
    public interface IManagerDashboardService
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalJobsAsync();
        Task<int> GetTotalInterviewsAsync();
        Task<Dictionary<string, int>> GetWeeklyInterviewCountAsync();
        Task<int> GetTotalRemindersTodayAsync();

    }
}