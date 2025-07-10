using AskHire_Backend.Interfaces.Repositories.ManagerRepositories;
using AskHire_Backend.Interfaces.Services.IManagerServices;
using AskHire_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.ManagerServices
{
    public class ManagerCandidateService : IManagerCandidateService
    {
        private readonly IManagerCandidateRepository _candidateRepository;

        public ManagerCandidateService(IManagerCandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        // Fetch all available vacancies
        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync()
        {
            return await _candidateRepository.GetVacanciesAsync();
        }

        public async Task<IEnumerable<object>> GetAllApplicationsAsync()
        {
            return await _candidateRepository.GetAllApplicationsAsync();
        }

        public async Task<object?> GetApplicationByIdAsync(Guid applicationId)
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
            if (!new[] { "longlist", "rejected", "pending" }.Contains(normalizedStatus))
            {
                throw new ArgumentException("Status must be 'longlist', 'rejected', or 'pending'.");
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

        // Added DownloadCVAsync method that returns IActionResult
        public async Task<IActionResult> DownloadCVAsync(Guid applicationId)
        {
            try
            {
                // Check if the application ID is valid
                if (applicationId == Guid.Empty)
                {
                    return new BadRequestObjectResult("Invalid application ID");
                }

                var fileBytes = await GetCVFileAsync(applicationId);

                // Check if the file bytes are null or empty
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return new NotFoundObjectResult($"CV file for application ID {applicationId} is empty or not found");
                }

                var fileName = await GetCVFileNameAsync(applicationId);

                // Check if the filename is valid
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"CV_{applicationId}.pdf";
                }

                var contentType = "application/pdf";

                // Try to determine content type from file extension if not pdf
                if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var extension = Path.GetExtension(fileName).ToLowerInvariant();
                    switch (extension)
                    {
                        case ".docx":
                            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            break;
                        case ".doc":
                            contentType = "application/msword";
                            break;
                            // Add more content types as needed
                    }
                }

                return new FileContentResult(fileBytes, contentType)
                {
                    FileDownloadName = fileName
                };
            }
            catch (FileNotFoundException ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new ObjectResult($"An error occurred while downloading the CV: {ex.Message}")
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<object> GetStatisticsAsync()
        {
            int longlist = await _candidateRepository.GetApplicationCountByStatusAsync("longlist");
            int rejected = await _candidateRepository.GetApplicationCountByStatusAsync("rejected");
            int pending = await _candidateRepository.GetApplicationCountByStatusAsync("pending");
            int total = longlist + rejected + pending;

            return new
            {
                Longlist = longlist,
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

            int longlist = await _candidateRepository.GetApplicationCountByVacancyAndStatusAsync(vacancyId, "longlist");
            int rejected = await _candidateRepository.GetApplicationCountByVacancyAndStatusAsync(vacancyId, "rejected");
            int pending = await _candidateRepository.GetApplicationCountByVacancyAndStatusAsync(vacancyId, "pending");
            int total = longlist + rejected + pending;

            return new
            {
                VacancyId = vacancyId,
                Longlist = longlist,
                Rejected = rejected,
                Pending = pending,
                Total = total
            };
        }
    }
}
