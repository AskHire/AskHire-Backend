using AskHire_Backend.Data.Repositories;
using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Services
{
    public class PreScreenTestService : IPreScreenTestService
    {
        private readonly IPreScreenTestRepository _repository;

        public PreScreenTestService(IPreScreenTestRepository repository)
        {
            _repository = repository;
        }

        public async Task<PreScreenTestDto?> GetVacancyInfo(Guid applicationId)
        {
            return await _repository.GetVacancyInfoByApplicationId(applicationId);
        }

        public async Task<PreScreenTestDto?> GetQuestions(Guid applicationId)
        {
            return await _repository.GetQuestionsByApplicationId(applicationId);
        }
    }
}
