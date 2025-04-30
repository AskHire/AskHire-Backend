namespace AskHire_Backend.Models.DTOs
{
    public class UserInterviewDetailsDto
    {
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid VacancyId { get; set; }
        public string VacancyName { get; set; } = string.Empty;
        public DateTime InterviewDate { get; set; }
        public TimeSpan InterviewTime { get; set; }
        public string InterviewInstructions { get; set; } = string.Empty;
    }
}