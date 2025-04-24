using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Services.Interfaces
{
    public interface IInterviewService
    {
        Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId);
    }
}
