// These are recommendations for your DTO models to fix the non-nullable property errors
// You can implement these changes if allowed

namespace AskHire_Backend.Models.DTOs
{
    public class TimeSlot
    {
        public required string StartTime { get; set; } // Add required modifier
        public required string EndTime { get; set; } // Add required modifier
    }

    public class SchedulePreviewRequest
    {
        public required string Date { get; set; } // Add required modifier
        public required string StartTime { get; set; } // Add required modifier
        public required string EndTime { get; set; } // Add required modifier
        public required string Duration { get; set; } // Add required modifier
    }

    public class InterviewScheduleRequest
    {
        public int ApplicationId { get; set; } // Value type, so not an issue
        public required string Date { get; set; } // Add required modifier
        public required string Time { get; set; } // Add required modifier
        public required string Instructions { get; set; } // Add required modifier
    }

    public class BatchScheduleRequest
    {
        public required List<InterviewScheduleRequest> Interviews { get; set; } // Add required modifier

        // Or initialize in constructor:
        // public BatchScheduleRequest()
        // {
        //     Interviews = new List<InterviewScheduleRequest>();
        // }
    }

    public class InterviewScheduleResult
    {
        public bool Success { get; set; } // Value type, so not an issue
        public int ApplicationId { get; set; } // Value type, so not an issue
        public int? InterviewId { get; set; } // Already nullable
        public required string Message { get; set; } // Add required modifier
    }
}