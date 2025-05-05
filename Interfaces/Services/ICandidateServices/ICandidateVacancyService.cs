using System.Collections.Generic;
using System.Threading.Tasks;
using AskHire_Backend.Models.DTOs;

namespace AskHire_Backend.Services
{
    public interface ICandidateVacancyService
    {
        Task<IEnumerable<CandidateVacancyDto>> GetJobWiseVacanciesAsync();
        Task<IEnumerable<CandidateVacancyDto>> GetMostAppliedVacanciesAsync();
        Task<IEnumerable<CandidateVacancyDto>> GetLatestVacanciesAsync();

    }
}
