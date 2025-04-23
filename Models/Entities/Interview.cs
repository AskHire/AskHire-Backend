using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AskHire_Backend.Models.Entities
{
    public class Interview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid InterviewId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        [Required]
        public string Instructions { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CandidateEmail { get; set; } = string.Empty;

        [ForeignKey("Application")]
        public Guid ApplicationId { get; set; }
        public required Application Application { get; set; }
    }
}