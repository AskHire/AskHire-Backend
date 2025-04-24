using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskHire_Backend.Models.Entities
{
    public class Reminder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ReminderId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateOnly Date { get; set; }
}

}
