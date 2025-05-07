using AskHire_Backend.Models.DTOs.CandidateDTOs;

namespace AskHire_Backend.Interfaces.Services.ICandidateServices
{
    public interface ICandidateInterviewService
    {
        Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId);
    }
}