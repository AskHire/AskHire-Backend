using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Repositories.Interfaces;
using AskHire_Backend.Services.Interfaces;

namespace AskHire_Backend.Services.Implementations
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepository;

        public InterviewService(IInterviewRepository interviewRepository)
        {
            _interviewRepository = interviewRepository;
        }

        public async Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId)
        {
            return await _interviewRepository.GetInterviewsByUserIdAsync(userId);
        }
    }
}