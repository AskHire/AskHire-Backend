using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public class ManagerDashboardService : IManagerDashboardService
    {
        private readonly IManagerDashboardRepository _managerDashboardRepository;

        public ManagerDashboardService(IManagerDashboardRepository managerDashboardRepository)
        {
            _managerDashboardRepository = managerDashboardRepository;
        }

        public async Task<int> GetTotalJobsAsync()
        {
            return await _managerDashboardRepository.GetTotalJobsAsync();
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _managerDashboardRepository.GetTotalUsersAsync();
        }

        public async Task<int> GetTotalInterviewsAsync()
        {
            return await _managerDashboardRepository.GetTotalInterviewsAsync();
        }

        public async Task<Dictionary<string, int>> GetWeeklyInterviewCountAsync()
        {
            return await _managerDashboardRepository.GetWeeklyInterviewCountAsync();
        }

        public async Task<int> GetTotalRemindersTodayAsync()
        {
            return await _managerDashboardRepository.GetTotalRemindersTodayAsync();
        }
    }
}
