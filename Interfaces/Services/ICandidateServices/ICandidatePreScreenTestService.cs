using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Interfaces.Services.ICandidateServices
{
    public interface ICandidatePreScreenTestService
    {
        Task<PreScreenTestDto?> GetVacancyInfo(Guid applicationId);
        Task<PreScreenTestDto?> GetQuestions(Guid applicationId);
    }
}
