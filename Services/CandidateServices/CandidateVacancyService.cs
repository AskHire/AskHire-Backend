using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Repositories;

namespace AskHire_Backend.Services
{
    public class CandidateVacancyService : ICandidateVacancyService
    {
        private readonly ICandidateVacancyRepository _candidateVacancyRepository;

        public CandidateVacancyService(ICandidateVacancyRepository candidateVacancyRepository)
        {
            _candidateVacancyRepository = candidateVacancyRepository;
        }

        public async Task<IEnumerable<CandidateVacancyDto>> GetJobWiseVacanciesAsync()
        {
            return await _candidateVacancyRepository.GetJobWiseVacanciesAsync();
        }
    }
}
