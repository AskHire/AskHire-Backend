using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Repositories
{
    public interface ICandidateVacancyRepository
    {
        Task<CandidateJobPagedResultDto<CandidateVacancyDto>> GetJobWiseVacanciesAsync(
            int pageNumber, int pageSize, string search, string sortOrder, bool isDemanded, bool isLatest);

        Task<IEnumerable<CandidateVacancyDto>> GetMostAppliedVacanciesAsync();
        Task<IEnumerable<CandidateVacancyDto>> GetLatestVacanciesAsync();
        Task<CandidateJobShowDto?> GetVacancyByIdAsync(Guid vacancyId);
    }
}