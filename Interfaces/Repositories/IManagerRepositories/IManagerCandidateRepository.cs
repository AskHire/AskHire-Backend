using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Repositories.ManagerRepositories
{
    public interface IManagerCandidateRepository
    {
        Task<IEnumerable<object>> GetAllApplicationsAsync();
        Task<object?> GetApplicationByIdAsync(Guid applicationId); // Made nullable
        Task<IEnumerable<object>> GetApplicationsByVacancyNameAsync(string vacancyName);
        Task<IEnumerable<object>> GetApplicationsByStatusAsync(string status);
        Task<bool> VacancyExistsAsync(Guid vacancyId);
        Task<int> GetApplicationCountByStatusAsync(string status);
        Task<int> GetApplicationCountByVacancyAndStatusAsync(Guid vacancyId, string status);
        Task<string?> GetCVPathAsync(Guid applicationId); // Made nullable
        Task<IEnumerable<Vacancy>> GetVacanciesAsync();
    }
}