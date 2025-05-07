using AskHire_Backend.Data.Entities;
using AskHire_Backend.Interfaces.Repositories.ManagerRepositories;
using AskHire_Backend.Interfaces.Services.IManagerServices;
using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.DTOs.CandidateDTOs;
using AskHire_Backend.Models.DTOs.ManagerDTOs;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Repositories.Interfaces;
using AskHire_Backend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.ManagerServices
{
    public class ManagerInterviewService : IManagerInterviewService
    {
        private readonly IManagerInterviewRepository _interviewRepository;
        private readonly IManagerEmailService _emailService;

        public ManagerInterviewService(IManagerInterviewRepository interviewRepository, IManagerEmailService emailService)
        {
            _interviewRepository = interviewRepository ?? throw new ArgumentNullException(nameof(interviewRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<bool> ScheduleInterviewAsync(ManagerInterviewScheduleRequestDTO interviewRequest)
        {
            if (interviewRequest == null)
            {
                return false;
            }

            // Get application with user details
            var application = await _interviewRepository.GetApplicationWithUserAsync(interviewRequest.ApplicationId);
            if (application == null || application.User == null)
            {
                return false;
            }

            // Convert string date, time, and duration to proper types
            if (!DateTime.TryParse(interviewRequest.Date, out DateTime interviewDate))
            {
                return false;
            }

            if (!TimeSpan.TryParse(interviewRequest.Time, out TimeSpan interviewTime))
            {
                return false;
            }

            if (!TimeSpan.TryParse(interviewRequest.Duration, out TimeSpan interviewDuration))
            {
                return false;
            }

            // Create interview entity
            var interview = new Interview
            {
                ApplicationId = application.ApplicationId,
                Application = application,
                Date = interviewDate,
                Time = interviewTime,
                Duration = interviewDuration,
                Interview_Instructions = interviewRequest.Interview_Instructions ?? string.Empty
            };

            // Save interview
            var createdInterview = await _interviewRepository.CreateInterviewAsync(interview);

            // Send email notification
            if (application.User?.Email == null)
            {
                return false;
            }

            return await _emailService.SendInterviewEmailAsync(application.User.Email, interview).ConfigureAwait(false);
        }

        public async Task<bool> UpdateInterviewAsync(Guid interviewId, ManagerInterviewScheduleRequestDTO interviewRequest)
        {
            if (interviewRequest == null)
            {
                return false;
            }

            // Get existing interview by application ID
            var existingInterview = await _interviewRepository.GetInterviewByApplicationIdAsync(interviewRequest.ApplicationId).ConfigureAwait(false);
            if (existingInterview == null)
            {
                return false;
            }

            // Get application with user details
            var application = await _interviewRepository.GetApplicationWithUserAsync(existingInterview.ApplicationId).ConfigureAwait(false);
            if (application == null || application.User == null)
            {
                return false;
            }

            // Convert string date, time, and duration to proper types
            if (!DateTime.TryParse(interviewRequest.Date, out DateTime interviewDate))
            {
                return false;
            }

            if (!TimeSpan.TryParse(interviewRequest.Time, out TimeSpan interviewTime))
            {
                return false;
            }

            if (!TimeSpan.TryParse(interviewRequest.Duration, out TimeSpan interviewDuration))
            {
                return false;
            }

            // Update interview entity
            existingInterview.Date = interviewDate;
            existingInterview.Time = interviewTime;
            existingInterview.Duration = interviewDuration;
            existingInterview.Interview_Instructions = interviewRequest.Interview_Instructions ?? string.Empty;

            // Save updated interview
            await _interviewRepository.UpdateInterviewAsync(existingInterview).ConfigureAwait(false);

            // Send email notification
            if (application.User?.Email == null)
            {
                return false;
            }

            return await _emailService.SendInterviewEmailAsync(application.User.Email, existingInterview).ConfigureAwait(false);
        }

        public async Task<Interview> GetInterviewByApplicationIdAsync(Guid applicationId)
        {
            return await _interviewRepository.GetInterviewByApplicationIdAsync(applicationId).ConfigureAwait(false);
        }

        public async Task<List<Interview>> GetAllInterviewsAsync()
        {
            return await _interviewRepository.GetAllInterviewsAsync().ConfigureAwait(false);
        }

        public async Task<List<UserInterviewDetailsDto>> GetInterviewsByUserIdAsync(Guid userId)
        {
            return await _interviewRepository.GetInterviewsByUserIdAsync(userId).ConfigureAwait(false);
        }

        
    }
}