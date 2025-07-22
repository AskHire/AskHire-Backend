using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public interface ICandidateVacancyService
    {
        Task<CandidateJobPagedResultDto<CandidateVacancyDto>> GetJobWiseVacanciesAsync(
            int pageNumber, int pageSize, string search, string sortOrder, bool isDemanded, bool isLatest,
            string workLocation, string workType); // New parameters

        Task<IEnumerable<CandidateVacancyDto>> GetMostAppliedVacanciesAsync();
        Task<IEnumerable<CandidateVacancyDto>> GetLatestVacanciesAsync();

        // MODIFIED METHOD SIGNATURE - userId is now nullable
        Task<(string status, CandidateJobShowDto? vacancy)> GetVacancyByIdAsync(Guid vacancyId, Guid? userId);
    }
}