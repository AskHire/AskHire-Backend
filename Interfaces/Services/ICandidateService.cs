using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Services
{
    public interface ICandidateService
    {
        Task<IEnumerable<object>> GetAllApplicationsAsync();
        Task<object> GetApplicationByIdAsync(Guid applicationId);
        Task<IEnumerable<object>> GetApplicationsByVacancyNameAsync(string vacancyName);
        Task<IEnumerable<object>> GetApplicationsByStatusAsync(string status);
        Task<byte[]> GetCVFileAsync(Guid applicationId);
        Task<string> GetCVFileNameAsync(Guid applicationId);
        Task<object> GetStatisticsAsync();
        Task<object> GetStatisticsByVacancyAsync(Guid vacancyId);
        Task<IEnumerable<Vacancy>> GetVacanciesAsync(); // Updated return type
    }
}
