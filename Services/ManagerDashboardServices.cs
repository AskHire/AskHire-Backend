using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Repositories.Interfaces;
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
    }
}