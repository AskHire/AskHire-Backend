namespace AskHire_Backend.Models.DTOs.AdminDTOs
{
    public class UserDTo
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "Candidate";

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Gender { get; set; }

        public string? DOB { get; set; }

        public string? NIC { get; set; }

        public string? MobileNumber { get; set; }

        public string? Address { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}
