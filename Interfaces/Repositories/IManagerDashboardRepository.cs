using AskHire_Backend.Data.Entities;
using AskHire_Backend.Models.Entities;

namespace AskHire_Backend.Repositories.Interfaces
{
    public interface IManagerDashboardRepository
    {

        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalJobsAsync();
        Task<int> GetTotalInterviewsAsync();
        Task<Dictionary<string, int>> GetWeeklyInterviewCountAsync();

    
    }
}