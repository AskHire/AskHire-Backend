using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Interfaces.Repositories.CandidateRepositories
{
    public interface ICandidatePreScreenTestRepository
    {
        Task<PreScreenTestDto?> GetVacancyInfoByApplicationId(Guid applicationId);
        Task<PreScreenTestDto?> GetQuestionsByApplicationId(Guid applicationId);
    }
}