using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using AskHire_Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public class CandidateVacancyService : ICandidateVacancyService
    {
        private readonly ICandidateVacancyRepository _candidateVacancyRepository;

        public CandidateVacancyService(ICandidateVacancyRepository candidateVacancyRepository)
        {
            _candidateVacancyRepository = candidateVacancyRepository;
        }

        // <<< MODIFIED >>>
        public async Task<CandidateJobPagedResultDto<CandidateVacancyDto>> GetJobWiseVacanciesAsync(
            int pageNumber, int pageSize, string search, string sortOrder, bool isDemanded, bool isLatest)
        {
            return await _candidateVacancyRepository.GetJobWiseVacanciesAsync(
                pageNumber, pageSize, search, sortOrder, isDemanded, isLatest);
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