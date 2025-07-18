using System;
using System.ComponentModel.DataAnnotations;

namespace AskHire_Backend.Models.DTOs
{
    public class ManagerInterviewScheduleRequestDTO
    {
        [Required]
        public Guid ApplicationId { get; set; }

        [Required]
        public string Date { get; set; } = string.Empty;

        [Required]
        public string Time { get; set; } = string.Empty;

        [Required]
        public string Duration { get; set; } = string.Empty;

        // Fixed: Only this property for instructions, public setter
        public string? Interview_Instructions { get; set; }

        // Optional interviewId for update operations
        public Guid? InterviewId { get; set; }
    }
}