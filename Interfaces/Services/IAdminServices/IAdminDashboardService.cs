using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.AdminDTOs;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;

namespace AskHire_Backend.Services
{
    public interface IAdminDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();

        Task<PaginatedResult<VacancyDashboardDto>> GetPagedVacancyTrackingAsync(PaginationQuery query);
    }
}
