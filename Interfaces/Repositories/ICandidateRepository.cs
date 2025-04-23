// Interfaces/Repositories/ICandidateRepository.cs
using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks;
using AskHire_Backend.Models;

namespace AskHire_Backend.Interfaces.Repositories
{
    public interface ICandidateRepository
    {
        Task<IEnumerable<object>> GetAllApplicationsAsync();
        Task<object> GetApplicationByIdAsync(Guid applicationId);
        Task<IEnumerable<object>> GetApplicationsByVacancyNameAsync(string vacancyName);
        Task<IEnumerable<object>> GetApplicationsByStatusAsync(string status);
        Task<bool> VacancyExistsAsync(Guid vacancyId);
        Task<int> GetApplicationCountByStatusAsync(string status);
        Task<int> GetApplicationCountByVacancyAndStatusAsync(Guid vacancyId, string status);
        Task<string> GetCVPathAsync(Guid applicationId);
        Task<IEnumerable<Vacancy>> GetVacanciesAsync();
    }
}