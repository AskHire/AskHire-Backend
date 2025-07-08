namespace AskHire_Backend.Models.DTOs
{
    public class CandidateJobShowDto
    {
        public required string VacancyName { get; set; }
        public required string Instructions { get; set; }
        public required string Experience { get; set; }
        public required string Education { get; set; }
        public required string NonTechnicalSkills { get; set; }
        public required DateTime StartDate { get; set; }
        public required string RequiredSkills { get; set; }
        public required DateTime EndDate { get; set; }
        public required string Description { get; set; }       // From JobRole
        public required string WorkType { get; set; }           // From JobRole
        public required string WorkLocation { get; set; }       // From JobRole
    }
}

