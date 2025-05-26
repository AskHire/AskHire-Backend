using AskHire_Backend.Models.DTOs.CandidateDTOs;

namespace AskHire_Backend.Interfaces.Services.ICandidateServices
{
    public interface ICandidateAnswerCheckService
    {
        Task<CheckAnswersResponseDto> CheckAnswersAsync(Guid applicationId, CheckAnswersRequest request);
    }
}
