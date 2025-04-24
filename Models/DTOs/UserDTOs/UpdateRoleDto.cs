namespace AskHire_Backend.Models.DTOs
{
    public class UpdateRoleDto
    {
        public Guid UserId { get; set; }
        public string NewRole { get; set; } = string.Empty;
    }
}
