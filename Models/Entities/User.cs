//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;

//namespace AskHire_Backend.Models.Entities
//{
//    public class User
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public Guid UserId { get; set; }

//        [MaxLength(50)]
//        public required string FirstName { get; set; }

//        [MaxLength(50)]
//        public required string LastName { get; set; }

//        [MaxLength(10)]
//        public required string Gender { get; set; }

//        [MaxLength(20)]
//        public required string Role { get; set; } = "Candidate";

//        [MaxLength(12)]
//        public required string NIC { get; set; }

//        public required DateTime DOB { get; set; }

//        [MaxLength(15)]
//        public required string MobileNumber { get; set; }

//        [EmailAddress]
//        public required string Email { get; set; }

//        [MaxLength(200)]
//        public required string Address { get; set; }

//        [MaxLength(50)]
//        public required string State { get; set; }

//        public string? RefreshToken { get; set; }
//        public DateTime? RefreshTokenExpiryTime { get; set; }

//    }
//}



using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AskHire_Backend.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string Role { get; set; } = "Candidate";
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public string? DOB { get; set; } = string.Empty;
        public string? NIC { get; set; } = string.Empty;
        public string? MobileNumber { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
    }
}


