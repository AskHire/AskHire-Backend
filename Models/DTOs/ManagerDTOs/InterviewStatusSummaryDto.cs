namespace AskHire_Backend.Models.DTOs.ManagerDTOs
{
    public class InterviewStatusSummaryDto
    {
        public int ScheduledCount { get; set; }
        public int YetToScheduleCount { get; set; }
        public int CompletedCount { get; set; } = 15; // Fixed value for now
    }
}
