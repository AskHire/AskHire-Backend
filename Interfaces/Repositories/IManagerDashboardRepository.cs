using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Repositories.Interfaces
{
    public interface IManagerDashboardRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalJobsAsync();
        Task<int> GetTotalInterviewsAsync();
        Task<Dictionary<string, int>> GetWeeklyInterviewCountAsync();
        Task<int> GetTotalRemindersTodayAsync();
    }
}