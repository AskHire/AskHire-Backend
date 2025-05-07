using AskHire_Backend.Interfaces.Repositories.CandidateRepositories;
using AskHire_Backend.Interfaces.Services.ICandidateServices;
using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Services.CandidateServices
{
    public class CandidatePreScreenTestService : ICandidatePreScreenTestService
    {
        private readonly ICandidatePreScreenTestRepository _repository;

        public CandidatePreScreenTestService(ICandidatePreScreenTestRepository repository)
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
