namespace AskHire_Backend.Models.DTOs
{
    public class CandidateVacancyDto
    {
        public Guid VacancyId { get; set; }
        public string VacancyName { get; set; }
        public string WorkType { get; set; }
        public string WorkLocation { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public DateTime EndDate { get; set; }
    }
}
