using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Data.Repositories
{
    public interface IPreScreenTestRepository
    {
        Task<PreScreenTestDto?> GetVacancyInfoByApplicationId(Guid applicationId);
        Task<PreScreenTestDto?> GetQuestionsByApplicationId(Guid applicationId);
    }
}