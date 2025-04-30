namespace AskHire_Backend.Repositories
{
    public interface IAdminDashboardRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalManagersAsync();
        Task<int> GetTotalCandidatesAsync();
    }
}
