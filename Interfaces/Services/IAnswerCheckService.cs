using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Services.Interfaces
{
    public interface IAnswerCheckService
    {
        Task<CheckAnswersResponseDto> CheckAnswersAsync(Guid applicationId, CheckAnswersRequest request);
    }
}
