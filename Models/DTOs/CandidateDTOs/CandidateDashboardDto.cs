namespace AskHire_Backend.DTOs
{
    public class CandidateDashboardDto
    {
        public Guid ApplicationId { get; set; }
        public string? DashboardStatus { get; set; }
        public string VacancyName { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }
        public string? JobRoleDescription { get; set; }
    }
}

