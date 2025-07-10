using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Interfaces.Services.IManagerServices
{
    public interface IManagerCandidateService
    {
        Task<IEnumerable<object>> GetAllApplicationsAsync();
        Task<object?> GetApplicationByIdAsync(Guid applicationId);
        Task<IEnumerable<object>> GetApplicationsByVacancyNameAsync(string vacancyName);
        Task<IEnumerable<object>> GetApplicationsByStatusAsync(string status);
        Task<byte[]> GetCVFileAsync(Guid applicationId);
        Task<string> GetCVFileNameAsync(Guid applicationId);
        Task<IActionResult> DownloadCVAsync(Guid applicationId); // Added missing method
        Task<object> GetStatisticsAsync();
        Task<object> GetStatisticsByVacancyAsync(Guid vacancyId);
        Task<IEnumerable<Vacancy>> GetVacanciesAsync();
    }
}