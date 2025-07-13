using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AskHire_Backend.Models.DTOs.ManagerDTOs
{
    public class ManagerLongListInterviewScheduleRequestDTO
    {
        [Required]
        public string Vacancy { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string StartTime { get; set; }
        [Required]
        public string EndTime { get; set; }
        [Required]
        public string InterviewDuration { get; set; }
        public string InterviewInstructions { get; set; }
        public bool SendEmail { get; set; } = true;
    }

    public class LongListInterviewResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ScheduledCount { get; set; }
        public int NotScheduledCount { get; set; }
        public List<string> FailedCandidates { get; set; } = new List<string>();
    }

    public class UnscheduledCandidateDTO
    {
        public Guid ApplicationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int CVMark { get; set; }
        public int PreScreenPassMark { get; set; }
        public string Status { get; set; }
        public string DashboardStatus { get; set; }
    }

    public class ScheduledInterviewDTO
    {
        public Guid InterviewId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Duration { get; set; }
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }
        public string Status { get; set; }
        public string DashboardStatus { get; set; }




    }
}