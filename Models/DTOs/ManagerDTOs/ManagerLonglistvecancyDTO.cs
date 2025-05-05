using System;

namespace AskHire_Backend.Models.DTOs
{
    public class ManagerLonglistvecancyDTO
    {
        public Guid VacancyId { get; set; }
        public string VacancyName { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string NonTechnicalSkills { get; set; } = string.Empty;
        public int CVPassMark { get; set; }
        public DateTime StartDate { get; set; }
        public string RequiredSkills { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }
        public int PreScreenPassMark { get; set; }
        public int Duration { get; set; }
        public int QuestionCount { get; set; }

        public Guid? JobId { get; set; }
        public string? JobRoleName { get; set; }
    }
}
