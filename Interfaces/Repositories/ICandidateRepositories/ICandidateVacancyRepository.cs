using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;

namespace AskHire_Backend.Repositories
{
    public interface ICandidateVacancyRepository
    {
        Task<CandidateJobPagedResultDto<CandidateVacancyDto>> GetJobWiseVacanciesAsync(int pageNumber, int pageSize, string search, string sortOrder);

        Task<IEnumerable<CandidateVacancyDto>> GetMostAppliedVacanciesAsync();
        Task<IEnumerable<CandidateVacancyDto>> GetLatestVacanciesAsync();

        Task<CandidateJobShowDto?> GetVacancyByIdAsync(Guid vacancyId);


    }
}
