using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskHire_Backend.Models.Entities
{
    public class JobRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid JobId { get; set; }

        public required string JobTitle { get; set; }
        public required string Description { get; set; }
        public required string WorkType { get; set; }
        public required string WorkLocation { get; set; }
    }
}
