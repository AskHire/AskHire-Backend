using System;

namespace AskHire_Backend.Models.DTOs
{
    public class InterviewScheduleRequestDTO
    {
        public Guid ApplicationId { get; set; }
        public required string Date { get; set; }
        public required string Time { get; set; }
        public required string Instructions { get; set; }
    }
}