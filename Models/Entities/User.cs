using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AskHire_Backend.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string Role { get; set; } = "Candidate";
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public DateTime? SignUpDate { get; set; }

        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public string? DOB { get; set; } = string.Empty;
        public string? NIC { get; set; } = string.Empty;
        public string? MobileNumber { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
    }
}


