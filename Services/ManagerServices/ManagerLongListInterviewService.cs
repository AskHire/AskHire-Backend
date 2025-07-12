using AskHire_Backend.Interfaces.Repositories.IManagerRepositories;
using AskHire_Backend.Interfaces.Services.IManagerServices;
using AskHire_Backend.Models.DTOs.ManagerDTOs;
using AskHire_Backend.Models.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskHire_Backend.Services.ManagerServices
{
    public class ManagerLongListInterviewService : IManagerLongListInterviewService
    {
        private readonly IManagerLongListInterviewRepository _repository;
        private readonly ILogger<ManagerLongListInterviewService> _logger;
        private readonly IManagerEmailService _emailService;

        // Constants for consistent status values
        private const string LONGLIST_STATUS = "Longlist";
        private const string PRE_SCREENING_DASHBOARD_STATUS = "Pre-Screening";
        private const string INTERVIEW_DASHBOARD_STATUS = "Interview";

        public ManagerLongListInterviewService(
            IManagerLongListInterviewRepository repository,
            ILogger<ManagerLongListInterviewService> logger,
            IManagerEmailService emailService)
        {
            _repository = repository;
            _logger = logger;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<LongListInterviewResultDTO> ScheduleLongListInterviewsAsync(ManagerLongListInterviewScheduleRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Starting to schedule interviews for vacancy: {Vacancy}", request.Vacancy);

                // Parse the vacancy ID or name
                Guid vacancyId;
                Vacancy vacancy;

                if (Guid.TryParse(request.Vacancy, out vacancyId))
                {
                    vacancy = await _repository.GetVacancyByIdAsync(vacancyId);
                }
                else
                {
                    vacancy = await _repository.GetVacancyByNameAsync(request.Vacancy);
                    if (vacancy != null)
                    {
                        vacancyId = vacancy.VacancyId;
                    }
                }

                if (vacancy == null)
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = $"Vacancy with ID/name '{request.Vacancy}' not found."
                    };
                }

                // Parse time values
                DateTime startTime;
                DateTime endTime;
                TimeSpan duration;

                if (!DateTime.TryParse($"{request.Date.ToString("yyyy-MM-dd")} {request.StartTime}", out startTime))
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "Invalid start time format."
                    };
                }

                if (!DateTime.TryParse($"{request.Date.ToString("yyyy-MM-dd")} {request.EndTime}", out endTime))
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "Invalid end time format."
                    };
                }

                if (!TimeSpan.TryParse(request.InterviewDuration, out duration))
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "Invalid duration format. Please use format like '00:30:00' for 30 minutes."
                    };
                }

                // Get unscheduled applications for this vacancy with Status = "Longlist" and DashboardStatus = "Pre-Screening"
                var unscheduledApplications = (await _repository.GetUnscheduledApplicationsAsync(vacancyId))
                    .Where(a => a.Status.Equals(LONGLIST_STATUS, StringComparison.OrdinalIgnoreCase) &&
                               a.DashboardStatus.Equals(PRE_SCREENING_DASHBOARD_STATUS, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!unscheduledApplications.Any())
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "No longlisted candidates found that need scheduling."
                    };
                }

                // Calculate total available time and number of slots
                TimeSpan totalAvailableTime = endTime - startTime;
                int totalSlots = (int)(totalAvailableTime.TotalMinutes / duration.TotalMinutes);

                if (totalSlots <= 0)
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "The selected time range is too short for the interview duration."
                    };
                }

                // Schedule interviews
                var result = new LongListInterviewResultDTO
                {
                    Success = true,
                    ScheduledCount = 0,
                    NotScheduledCount = 0,
                    FailedCandidates = new List<string>()
                };

                DateTime currentTime = startTime;
                int scheduledCount = 0;

                foreach (var application in unscheduledApplications)
                {
                    if (scheduledCount >= totalSlots || currentTime.Add(duration) > endTime)
                    {
                        // No more slots available
                        result.NotScheduledCount++;
                        result.FailedCandidates.Add($"{application.User.FirstName} {application.User.LastName} - No available slots");
                        continue;
                    }

                    var interview = new Interview
                    {
                        InterviewId = Guid.NewGuid(),
                        Date = request.Date,
                        Time = currentTime.TimeOfDay,
                        Duration = duration,
                        Interview_Instructions = request.InterviewInstructions ?? "Please be on time for your interview.",
                        ApplicationId = application.ApplicationId,
                        Application = application
                    };

                    bool saved = await _repository.SaveInterviewAsync(interview);
                    if (saved)
                    {
                        // FIXED: Use consistent dashboard status
                        application.DashboardStatus = INTERVIEW_DASHBOARD_STATUS;
                        await _repository.UpdateApplicationAsync(application);

                        // Check if SendEmail is requested and send email notification
                        bool emailSent = true;
                        if (request.SendEmail && application.User?.Email != null)
                        {
                            emailSent = await _emailService.SendInterviewEmailAsync(application.User.Email, interview);
                            if (!emailSent)
                            {
                                _logger.LogWarning("Failed to send email to {Email} for interview ID {InterviewId}",
                                    application.User.Email, interview.InterviewId);
                            }
                        }

                        result.ScheduledCount++;
                        scheduledCount++;
                        currentTime = currentTime.Add(duration);
                    }
                    else
                    {
                        result.NotScheduledCount++;
                        result.FailedCandidates.Add($"{application.User.FirstName} {application.User.LastName} - Database error");
                    }
                }

                string emailStatus = request.SendEmail ? "Email notifications have been sent to scheduled candidates." : "";
                result.Message = $"Successfully scheduled {result.ScheduledCount} interviews. {result.NotScheduledCount} candidates could not be scheduled. {emailStatus}";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling interviews");
                return new LongListInterviewResultDTO
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<LongListInterviewResultDTO> ScheduleLongListedCandidatesAsync(ManagerLongListInterviewScheduleRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Starting to schedule interviews for long-listed candidates in vacancy: {Vacancy}", request.Vacancy);

                // Parse the vacancy ID or name
                Guid vacancyId;
                Vacancy vacancy;

                if (Guid.TryParse(request.Vacancy, out vacancyId))
                {
                    vacancy = await _repository.GetVacancyByIdAsync(vacancyId);
                }
                else
                {
                    vacancy = await _repository.GetVacancyByNameAsync(request.Vacancy);
                    if (vacancy != null)
                    {
                        vacancyId = vacancy.VacancyId;
                    }
                }

                if (vacancy == null)
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = $"Vacancy with ID/name '{request.Vacancy}' not found."
                    };
                }

                // Parse time values
                DateTime startTime;
                DateTime endTime;
                TimeSpan duration;

                if (!DateTime.TryParse($"{request.Date.ToString("yyyy-MM-dd")} {request.StartTime}", out startTime))
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "Invalid start time format."
                    };
                }

                if (!DateTime.TryParse($"{request.Date.ToString("yyyy-MM-dd")} {request.EndTime}", out endTime))
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "Invalid end time format."
                    };
                }

                if (!TimeSpan.TryParse(request.InterviewDuration, out duration))
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "Invalid duration format. Please use format like '00:30:00' for 30 minutes."
                    };
                }

                // Get longlisted applications for this vacancy with Status = "Longlist" and DashboardStatus = "Pre-Screening"
                var longlistedApplications = (await _repository.GetLongListApplicationsAsync(vacancyId))
                    .Where(a => a.Status.Equals(LONGLIST_STATUS, StringComparison.OrdinalIgnoreCase) &&
                               a.DashboardStatus.Equals(PRE_SCREENING_DASHBOARD_STATUS, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!longlistedApplications.Any())
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "No longlisted candidates found that need scheduling."
                    };
                }

                // Calculate total available time and number of slots
                TimeSpan totalAvailableTime = endTime - startTime;
                int totalSlots = (int)(totalAvailableTime.TotalMinutes / duration.TotalMinutes);

                if (totalSlots <= 0)
                {
                    return new LongListInterviewResultDTO
                    {
                        Success = false,
                        Message = "The selected time range is too short for the interview duration."
                    };
                }

                // Schedule interviews
                var result = new LongListInterviewResultDTO
                {
                    Success = true,
                    ScheduledCount = 0,
                    NotScheduledCount = 0,
                    FailedCandidates = new List<string>()
                };

                DateTime currentTime = startTime;
                int scheduledCount = 0;

                foreach (var application in longlistedApplications)
                {
                    if (scheduledCount >= totalSlots || currentTime.Add(duration) > endTime)
                    {
                        // No more slots available
                        result.NotScheduledCount++;
                        result.FailedCandidates.Add($"{application.User.FirstName} {application.User.LastName} - No available slots");
                        continue;
                    }

                    var interview = new Interview
                    {
                        InterviewId = Guid.NewGuid(),
                        Date = request.Date,
                        Time = currentTime.TimeOfDay,
                        Duration = duration,
                        Interview_Instructions = request.InterviewInstructions ?? "Please be on time for your interview.",
                        ApplicationId = application.ApplicationId,
                        Application = application
                    };

                    bool saved = await _repository.SaveInterviewAsync(interview);
                    if (saved)
                    {
                        // FIXED: Use consistent dashboard status
                        application.DashboardStatus = INTERVIEW_DASHBOARD_STATUS;
                        await _repository.UpdateApplicationAsync(application);

                        // Check if SendEmail is requested and send email notification
                        bool emailSent = true;
                        if (request.SendEmail && application.User?.Email != null)
                        {
                            emailSent = await _emailService.SendInterviewEmailAsync(application.User.Email, interview);
                            if (!emailSent)
                            {
                                _logger.LogWarning("Failed to send email to {Email} for interview ID {InterviewId}",
                                    application.User.Email, interview.InterviewId);
                            }
                        }

                        result.ScheduledCount++;
                        scheduledCount++;
                        currentTime = currentTime.Add(duration);
                    }
                    else
                    {
                        result.NotScheduledCount++;
                        result.FailedCandidates.Add($"{application.User.FirstName} {application.User.LastName} - Database error");
                    }
                }

                string emailStatus = request.SendEmail ? "Email notifications have been sent to scheduled candidates." : "";
                result.Message = $"Successfully scheduled {result.ScheduledCount} interviews. {result.NotScheduledCount} candidates could not be scheduled. {emailStatus}";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling interviews for longlisted candidates: {Error}", ex.Message);
                return new LongListInterviewResultDTO
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<List<UnscheduledCandidateDTO>> GetUnscheduledCandidatesAsync(Guid vacancyId)
        {
            try
            {
                // Get unscheduled applications with Status = "Longlist" and DashboardStatus = "Pre-Screening"
                var unscheduledApplications = (await _repository.GetUnscheduledApplicationsAsync(vacancyId))
                    .Where(a => a.Status.Equals(LONGLIST_STATUS, StringComparison.OrdinalIgnoreCase) &&
                               a.DashboardStatus.Equals(PRE_SCREENING_DASHBOARD_STATUS, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var candidateDTOs = unscheduledApplications.Select(a => new UnscheduledCandidateDTO
                {
                    ApplicationId = a.ApplicationId,
                    FirstName = a.User.FirstName,
                    LastName = a.User.LastName,
                    Email = a.User.Email,
                    CVMark = a.CV_Mark,
                    PreScreenPassMark = a.Pre_Screen_PassMark,
                    Status = a.Status,
                    DashboardStatus = a.DashboardStatus
                }).ToList();

                return candidateDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unscheduled candidates");
                throw;
            }
        }

        public async Task<List<UnscheduledCandidateDTO>> GetLongListedCandidatesAsync(Guid vacancyId)
        {
            try
            {
                // Get longlisted applications with Status = "Longlist" and DashboardStatus = "Pre-Screening"
                var longlistedApplications = (await _repository.GetLongListApplicationsAsync(vacancyId))
                    .Where(a => a.Status.Equals(LONGLIST_STATUS, StringComparison.OrdinalIgnoreCase) &&
                               a.DashboardStatus.Equals(PRE_SCREENING_DASHBOARD_STATUS, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var candidateDTOs = longlistedApplications.Select(a => new UnscheduledCandidateDTO
                {
                    ApplicationId = a.ApplicationId,
                    FirstName = a.User.FirstName,
                    LastName = a.User.LastName,
                    Email = a.User.Email,
                    CVMark = a.CV_Mark,
                    PreScreenPassMark = a.Pre_Screen_PassMark,
                    Status = a.Status,
                    DashboardStatus = a.DashboardStatus
                }).ToList();

                return candidateDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting longlisted candidates");
                throw;
            }
        }

        public async Task<List<ScheduledInterviewDTO>> GetScheduledInterviewsAsync(Guid vacancyId)
        {
            try
            {
                // FIXED: Get scheduled interviews for applications with Status = "Longlist" and DashboardStatus = "Interview"
                var interviews = (await _repository.GetScheduledInterviewsAsync(vacancyId))
                    .Where(i => i.Application.Status.Equals(LONGLIST_STATUS, StringComparison.OrdinalIgnoreCase) &&
                               i.Application.DashboardStatus.Equals(INTERVIEW_DASHBOARD_STATUS, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var interviewDTOs = interviews.Select(i => new ScheduledInterviewDTO
                {
                    InterviewId = i.InterviewId,
                    Date = i.Date,
                    Time = i.Time,
                    Duration = i.Duration,
                    CandidateName = $"{i.Application.User.FirstName} {i.Application.User.LastName}",
                    CandidateEmail = i.Application.User.Email,
                    Status = i.Application.Status,
                    DashboardStatus = i.Application.DashboardStatus
                }).ToList();

                return interviewDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scheduled interviews");
                throw;
            }
        }
    }
}