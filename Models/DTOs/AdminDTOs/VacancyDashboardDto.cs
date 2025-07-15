namespace AskHire_Backend.Models.DTOs.AdminDTOs
{
    public class VacancyDashboardDto
    {
        public string VacancyName { get; set; } = string.Empty;
        public string JobRole { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ApplicationsCount { get; set; }
    }
}