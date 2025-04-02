using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AskHire_Backend.Models.Entities
{
    public class Application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApplicationId { get; set; }
        public required int CV_Mark { get; set; }
        public required string CVFilePath { get; set; }
        public required int Pre_Screen_PassMark { get; set; }
        public required string Status { get; set; }


        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public required User User { get; set; }

        [ForeignKey("Vacancy")]
        public Guid VacancyId { get; set; }
        public required Vacancy Vacancy { get; set; }
    }


}

