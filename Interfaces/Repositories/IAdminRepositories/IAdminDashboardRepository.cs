namespace AskHire_Backend.Interfaces.Repositories.AdminRepositories
{
    public interface IAdminDashboardRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalManagersAsync();
        Task<int> GetTotalCandidatesAsync();
    }
}
