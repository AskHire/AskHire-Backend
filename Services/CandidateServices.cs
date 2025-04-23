// Services/CandidateService.cs
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Services;
using AskHire_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidateService(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }


        // Fetch all available vacancies
        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync()
        {
            return await _candidateRepository.GetVacanciesAsync(); // Fetch vacancies from repository
        }



        public async Task<IEnumerable<object>> GetAllApplicationsAsync()
        {
            return await _candidateRepository.GetAllApplicationsAsync();
        }

        public async Task<object> GetApplicationByIdAsync(Guid applicationId)
        {
            return await _candidateRepository.GetApplicationByIdAsync(applicationId);
        }

        public async Task<IEnumerable<object>> GetApplicationsByVacancyNameAsync(string vacancyName)
        {
            if (string.IsNullOrEmpty(vacancyName))
            {
                throw new ArgumentException("Vacancy name must not be empty.");
            }

            return await _candidateRepository.GetApplicationsByVacancyNameAsync(vacancyName);
        }

        public async Task<IEnumerable<object>> GetApplicationsByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("Status must not be empty.");
            }

            string normalizedStatus = status.ToLower();
            if (!new[] { "qualified", "rejected", "pending" }.Contains(normalizedStatus))
            {
                throw new ArgumentException("Status must be 'qualified', 'rejected', or 'pending'.");
            }

            return await _candidateRepository.GetApplicationsByStatusAsync(normalizedStatus);
        }

        public async Task<byte[]> GetCVFileAsync(Guid applicationId)
        {
            var cvPath = await _candidateRepository.GetCVPathAsync(applicationId);

            if (string.IsNullOrEmpty(cvPath))
            {
                throw new FileNotFoundException("CV file not available.");
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedCVs", cvPath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("CV file not found on server.");
            }

            return await File.ReadAllBytesAsync(fullPath);
        }

        public async Task<string> GetCVFileNameAsync(Guid applicationId)
        {
            var cvPath = await _candidateRepository.GetCVPathAsync(applicationId);

            if (string.IsNullOrEmpty(cvPath))
            {
                throw new FileNotFoundException("CV file not available.");
            }

            return Path.GetFileName(cvPath);
        }

        public async Task<object> GetStatisticsAsync()
        {
            int qualified = await _candidateRepository.GetApplicationCountByStatusAsync("qualified");
            int rejected = await _candidateRepository.GetApplicationCountByStatusAsync("rejected");
            int pending = await _candidateRepository.GetApplicationCountByStatusAsync("pending");
            int total = qualified + rejected + pending;

            return new
            {
                Qualified = qualified,
                Rejected = rejected,
                Pending = pending,
                Total = total
            };
        }

        public async Task<object> GetStatisticsByVacancyAsync(Guid vacancyId)
        {
            bool vacancyExists = await _candidateRepository.VacancyExistsAsync(vacancyId);
            if (!vacancyExists)
            {
                throw new KeyNotFoundException($"No vacancy found with id {vacancyId}");
            }

            int qualified = await _candidateRepository.GetApplicationCountByVacancyAndStatusAsync(vacancyId, "qualified");
            int rejected = await _candidateRepository.GetApplicationCountByVacancyAndStatusAsync(vacancyId, "rejected");
            int pending = await _candidateRepository.GetApplicationCountByVacancyAndStatusAsync(vacancyId, "pending");
            int total = qualified + rejected + pending;

            return new
            {
                VacancyId = vacancyId,
                Qualified = qualified,
                Rejected = rejected,
                Pending = pending,
                Total = total
            };
        }
    }
}