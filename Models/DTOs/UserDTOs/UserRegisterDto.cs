namespace AskHire_Backend.Models.DTOs
{
    public class UserRegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public DateTime? SignUpDate { get; set; }
        public string? Gender { get; set; }
        public string? DOB { get; set; }
        public string? NIC { get; set; }
        public string? MobileNumber { get; set; }
        public string? Address { get; set; }
    }
}