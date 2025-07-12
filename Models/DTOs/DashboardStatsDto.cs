namespace AskHire_Backend.Models.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalCandidates { get; set; }
        public int TotalManagers { get; set; }
        public int TotalJobs { get; set; }

        public List<int>? SignupsPerMonth { get; set; }
        public Dictionary<string, int> UsersByAgeGroup { get; set; } = new();
    }
}
