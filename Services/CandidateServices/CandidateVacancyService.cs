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
            _candidateVacancyRepository = candidateVacancyRepository ?? throw new ArgumentNullException(nameof(candidateVacancyRepository));
        }

        public Task<CandidateJobPagedResultDto<CandidateVacancyDto>> GetJobWiseVacanciesAsync(
            int pageNumber, int pageSize, string search, string sortOrder, bool isDemanded, bool isLatest,
            string workLocation, string workType) // New parameters
        {
            // Delegate the call directly to the repository
            return _candidateVacancyRepository.GetJobWiseVacanciesAsync(pageNumber, pageSize, search, sortOrder, isDemanded, isLatest, workLocation, workType); // Pass new parameters
        }

        public Task<IEnumerable<CandidateVacancyDto>> GetMostAppliedVacanciesAsync()
        {
            // Delegate the call directly to the repository
            return _candidateVacancyRepository.GetMostAppliedVacanciesAsync();
        }

        public Task<IEnumerable<CandidateVacancyDto>> GetLatestVacanciesAsync()
        {
            // Delegate the call directly to the repository
            return _candidateVacancyRepository.GetLatestVacanciesAsync();
        }

        // MODIFIED METHOD SIGNATURE - userId is now nullable
        public Task<(string status, CandidateJobShowDto? vacancy)> GetVacancyByIdAsync(Guid vacancyId, Guid? userId)
        {
            // Delegate the call directly to the repository
            return _candidateVacancyRepository.GetVacancyByIdAsync(vacancyId, userId);
        }
    }
}