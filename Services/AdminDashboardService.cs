using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Repositories;

namespace AskHire_Backend.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _adminRepository;

        public AdminDashboardService(IAdminDashboardRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalUsers = await _adminRepository.GetTotalUsersAsync();
            var totalManagers = await _adminRepository.GetTotalManagersAsync();
            var totalCandidates = await _adminRepository.GetTotalCandidatesAsync();

            return new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalManagers = totalManagers,
                TotalCandidates = totalCandidates,
                TotalJobs = 0  // You can count jobs too if you want (optional)
            };
        }

    }
}
