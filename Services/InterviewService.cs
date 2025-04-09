using AskHire_Backend.Models.DTOs;
using AskHire_Backend.Models.Entities;
using AskHire_Backend.Repositories.Interfaces;
using AskHire_Backend.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace AskHire_Backend.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly IEmailService _emailService;

        public InterviewService(IInterviewRepository interviewRepository, IEmailService emailService)
        {
            _interviewRepository = interviewRepository ?? throw new ArgumentNullException(nameof(interviewRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<bool> ScheduleInterviewAsync(InterviewScheduleRequest interviewRequest)
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

            // Convert string date and time to proper types
            if (!DateTime.TryParse(interviewRequest.Date, out DateTime interviewDate))
            {
                return false;
            }

            if (!TimeSpan.TryParse(interviewRequest.Time, out TimeSpan interviewTime))
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
                Instructions = interviewRequest.Instructions ?? string.Empty,
                CandidateEmail = application.User?.Email ?? string.Empty
            };

            // Save interview
            var createdInterview = await _interviewRepository.CreateInterviewAsync(interview);

            // Send email notification
            if (string.IsNullOrEmpty(interview.CandidateEmail))
            {
                return false;
            }

            return await _emailService.SendInterviewEmailAsync(interview.CandidateEmail, interview);
        }
    }
}