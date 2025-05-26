using AskHire_Backend.Models.DTOs.CandidateDTOs;

namespace AskHire_Backend.Interfaces.Repositories.CandidateRepositories
{
    public interface ICandidateInterviewRepository
    {
        Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId);
    }
}