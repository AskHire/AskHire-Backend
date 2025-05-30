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

        public async Task<IEnumerable<CandidateVacancyDto>> GetMostAppliedVacanciesAsync()
        {
            return await _candidateVacancyRepository.GetMostAppliedVacanciesAsync();
        }

        public async Task<IEnumerable<CandidateVacancyDto>> GetLatestVacanciesAsync()
        {
            return await _candidateVacancyRepository.GetLatestVacanciesAsync();
        }

        public async Task<CandidateJobShowDto?> GetVacancyByIdAsync(Guid vacancyId)
        {
            return await _candidateVacancyRepository.GetVacancyByIdAsync(vacancyId);
        }


    }
}
