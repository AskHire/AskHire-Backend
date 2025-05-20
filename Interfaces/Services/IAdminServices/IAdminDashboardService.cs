using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Services
{
    public interface IAdminDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
