using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Repositories.Interfaces
{
    public interface IInterviewRepository
    {
        Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId);
    }
}