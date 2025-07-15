using AskHire_Backend.Models.DTOs.AdminDTOs;
using AskHire_Backend.Models.DTOs.AdminDTOs.PaginationDTOs;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Repositories.AdminRepositories
{
    public interface IAdminDashboardRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalManagersAsync();
        Task<int> GetTotalCandidatesAsync();

        Task<int> GetTotalJobsAsync();
        Task<List<int>> GetMonthlySignupsAsync();
        Task<Dictionary<string, int>> GetUsersByAgeGroupAsync();
        Task<PaginatedResult<VacancyDashboardDto>> GetPagedVacancyTrackingTableAsync(PaginationQuery query);

    }
}
