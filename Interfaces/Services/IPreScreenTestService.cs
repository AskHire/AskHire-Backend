using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Services
{
    public interface IPreScreenTestService
    {
        Task<PreScreenTestDto?> GetVacancyInfo(Guid applicationId);
        Task<PreScreenTestDto?> GetQuestions(Guid applicationId);
    }
}
