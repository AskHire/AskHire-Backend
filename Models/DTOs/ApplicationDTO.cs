using System;

namespace AskHire_Backend.Models.DTOs
{
    public class CandidateStatisticsDto
    {
        public Guid? VacancyId { get; set; }
        public int Qualified { get; set; }
        public int Rejected { get; set; }
        public int Pending { get; set; }
        public int Total { get; set; }
    }
}