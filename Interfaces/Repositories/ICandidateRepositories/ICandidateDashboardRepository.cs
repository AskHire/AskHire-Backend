using AskHire_Backend.DTOs;

namespace AskHire_Backend.Repositories.Interfaces
{
    public interface ICandidateDashboardRepository
    {
        Task<List<CandidateDashboardDto>> GetApplicationsByUserIdAsync(Guid userId);
        Task<bool> DeleteCandidateApplicationAsync(Guid applicationId);

    }
}

