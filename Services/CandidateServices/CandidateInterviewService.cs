using AskHire_Backend.Interfaces.Repositories.CandidateRepositories;
using AskHire_Backend.Interfaces.Services.ICandidateServices;
using AskHire_Backend.Models.DTOs.CandidateDTOs;

namespace AskHire_Backend.Services.CandidateServices
{
    public class CandidateInterviewService : ICandidateInterviewService
    {
        private readonly ICandidateInterviewRepository _interviewRepository;

        public CandidateInterviewService(ICandidateInterviewRepository interviewRepository)
        {
            _interviewRepository = interviewRepository;
        }

        public async Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId)
        {
            return await _interviewRepository.GetInterviewsByUserIdAsync(userId);
        }
    }
}