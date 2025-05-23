using AskHire_Backend.DTOs;
using AskHire_Backend.Repositories.Interfaces;
using AskHire_Backend.Services.Interfaces;

namespace AskHire_Backend.Services
{
    public class CandidateDashboardService : ICandidateDashboardService
    {
        private readonly ICandidateDashboardRepository _repository;

        public CandidateDashboardService(ICandidateDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CandidateDashboardDto>> GetCandidateApplicationsAsync(Guid userId)
        {
            return await _repository.GetApplicationsByUserIdAsync(userId);
        }

        public async Task<bool> DeleteCandidateApplicationAsync(Guid applicationId)
        {
            return await _repository.DeleteCandidateApplicationAsync(applicationId);
        }
    }


}
