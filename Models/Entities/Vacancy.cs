using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskHire_Backend.Models.Entities
{
    public class Vacancy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid VacancyId { get; set; }


        public required string VacancyName { get; set; }

        public required string Instructions { get; set; }

        public required string Experience { get; set; }

        public required string Education { get; set; }
        public required string NonTechnicalSkills { get; set; }
        public required int CVPassMark { get; set; }
        public required DateTime StartDate { get; set; }
        public required string RequiredSkills { get; set; }
        public required DateTime EndDate { get; set; }
        public required int PreScreenPassMark { get; set; }
        public required int Duration { get; set; }
        public required int QuestionCount { get; set; }

        [ForeignKey("JobRole")]
        public Guid? JobId { get; set; }
        public JobRole? JobRole { get; set; }

        public virtual ICollection<Application> Applies { get; set; } = new List<Application>();
    }
}
