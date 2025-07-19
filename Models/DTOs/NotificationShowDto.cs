namespace AskHire_Backend.Models.DTOs
{
    public class NotificationShowDto
    {
        public Guid NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string? Status { get; set; }
    }
}
