using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskHire_Backend.Models.Entities
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NotificationId { get; set; }

        public required string Message { get; set; }
        public required DateTime Time { get; set; }
        public required string Type { get; set; }
        public required string Subject { get; set; }

        public string? Status { get; set; }    }
}
