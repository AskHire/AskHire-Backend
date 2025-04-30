using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace AskHire_Backend.Models.Entities
{
    public class Interview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid InterviewId { get; set; }
        public required DateTime Date { get; set; }
        public required TimeSpan Time { get; set; }
        public required TimeSpan Duration { get; set; }
        public required string Interview_Instructions { get; set; }

        [ForeignKey("Application")]
        public Guid ApplicationId { get; set; }
        public required Application Application { get; set; }
    }
}