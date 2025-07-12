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

    }
}
