using AskHire_Backend.Interfaces.Repositories.AdminRepositories;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.AdminDTOs;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;

namespace AskHire_Backend.Services.AdminServices
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
            var totalJobs = await _adminRepository.GetTotalJobsAsync();
            var monthlySignups = await _adminRepository.GetMonthlySignupsAsync();
            var usersByAgeGroup = await _adminRepository.GetUsersByAgeGroupAsync();

            return new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalManagers = totalManagers,
                TotalCandidates = totalCandidates,
                TotalJobs = totalJobs,
                SignupsPerMonth = monthlySignups,
                UsersByAgeGroup = usersByAgeGroup
            };
        }

        public async Task<PaginatedResult<VacancyDashboardDto>> GetPagedVacancyTrackingAsync(PaginationQuery query)
        {
            return await _adminRepository.GetPagedVacancyTrackingTableAsync(query);
        }

    }
}
