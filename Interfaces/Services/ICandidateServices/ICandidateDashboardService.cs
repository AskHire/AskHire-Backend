using AskHire_Backend.DTOs;

namespace AskHire_Backend.Services.Interfaces
{
    public interface ICandidateDashboardService
    {
        Task<List<CandidateDashboardDto>> GetCandidateApplicationsAsync(Guid userId);

        Task<bool> DeleteCandidateApplicationAsync(Guid applicationId);

    }
}
