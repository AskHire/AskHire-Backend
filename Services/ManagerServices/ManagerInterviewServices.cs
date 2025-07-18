// File: Services/ManagerInterviewService.cs

using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories;
using AskHire_Backend.Interfaces.Repositories.ManagerRepositories;
using AskHire_Backend.Interfaces.Services.IManagerServices;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using AskHire_Backend.Models.DTOs.ManagerDTOs;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.ManagerServices
{
    public class ManagerInterviewService : IManagerInterviewService
    {
        private readonly AskHire_Backend.Interfaces.Repositories.ManagerRepositories.IManagerInterviewRepository _interviewRepository;
        private readonly IManagerEmailService _emailService;
        private readonly IReminderRepository _reminderRepository;

        public ManagerInterviewService(
            AskHire_Backend.Interfaces.Repositories.ManagerRepositories.IManagerInterviewRepository interviewRepository,
            IManagerEmailService emailService,
            IReminderRepository reminderRepository)
        {
            _interviewRepository = interviewRepository ?? throw new ArgumentNullException(nameof(interviewRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _reminderRepository = reminderRepository ?? throw new ArgumentNullException(nameof(reminderRepository));
        }

        public async Task<bool> ScheduleInterviewAsync(ManagerInterviewScheduleRequestDTO interviewRequest)
        {
            if (interviewRequest == null)
                return false;

            // Load Application with User and Vacancy
            var application = await _interviewRepository.GetApplicationWithUserAsync(interviewRequest.ApplicationId);

            if (application == null || application.User == null || application.Vacancy == null)
                return false;

            if (!DateTime.TryParse(interviewRequest.Date, out DateTime interviewDate) ||
                !TimeSpan.TryParse(interviewRequest.Time, out TimeSpan interviewTime) ||
                !TimeSpan.TryParse(interviewRequest.Duration, out TimeSpan interviewDuration))
                return false;

            var interview = new Interview
            {
                ApplicationId = application.ApplicationId,
                Application = application,
                Date = interviewDate,
                Time = interviewTime,
                Duration = interviewDuration,
                Interview_Instructions = interviewRequest.Interview_Instructions ?? string.Empty
            };

            var createdInterview = await _interviewRepository.CreateInterviewAsync(interview);

            // Set DashboardStatus to "Interview" after scheduling
            application.DashboardStatus = "Interview";
            await _interviewRepository.UpdateApplicationAsync(application);

            // ? No NullReferenceException because Vacancy is included now
            var reminder = new Reminder
            {
                Title = $"{application.ApplicationId} -> {application.Vacancy.VacancyName}",
                Description = interviewTime.ToString(),
                Date = DateOnly.FromDateTime(interviewDate)
            };

            await _reminderRepository.CreateReminderAsync(reminder);

            if (string.IsNullOrEmpty(application.User?.Email))
                return false;

            return await _emailService.SendInterviewEmailAsync(application.User.Email, interview);
        }

        public async Task<bool> UpdateInterviewAsync(Guid interviewId, ManagerInterviewScheduleRequestDTO interviewRequest)
        {
            if (interviewRequest == null)
                return false;

            var existingInterview = await _interviewRepository.GetInterviewByApplicationIdAsync(interviewRequest.ApplicationId);

            if (existingInterview == null)
                return false;

            var application = await _interviewRepository.GetApplicationWithUserAsync(existingInterview.ApplicationId);

            if (application == null || application.User == null)
                return false;

            if (!DateTime.TryParse(interviewRequest.Date, out DateTime interviewDate) ||
                !TimeSpan.TryParse(interviewRequest.Time, out TimeSpan interviewTime) ||
                !TimeSpan.TryParse(interviewRequest.Duration, out TimeSpan interviewDuration))
                return false;

            existingInterview.Date = interviewDate;
            existingInterview.Time = interviewTime;
            existingInterview.Duration = interviewDuration;
            existingInterview.Interview_Instructions = interviewRequest.Interview_Instructions ?? string.Empty;

            await _interviewRepository.UpdateInterviewAsync(existingInterview);

            // Set DashboardStatus to "Interview" after updating
            application.DashboardStatus = "Interview";
            await _interviewRepository.UpdateApplicationAsync(application);

            if (string.IsNullOrEmpty(application.User?.Email))
                return false;

            return await _emailService.SendInterviewEmailAsync(application.User.Email, existingInterview);
        }

        public async Task<Interview> GetInterviewByApplicationIdAsync(Guid applicationId)
        {
            return await _interviewRepository.GetInterviewByApplicationIdAsync(applicationId);
        }

        public async Task<List<Interview>> GetAllInterviewsAsync()
        {
            return await _interviewRepository.GetAllInterviewsAsync();
        }

        public async Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId)
        {
            return await _interviewRepository.GetInterviewsByUserIdAsync(userId);
        }
    }
}
