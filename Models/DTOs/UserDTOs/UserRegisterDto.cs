using System.ComponentModel.DataAnnotations;

namespace AskHire_Backend.Models.DTOs
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s.-]+$", ErrorMessage = "First name can only contain letters, spaces, hyphens, and periods.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s.-]+$", ErrorMessage = "Last name can only contain letters, spaces, hyphens, and periods.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string? Gender { get; set; }

        // DOB as string in YYYY-MM-DD format
        [Required(ErrorMessage = "Date of Birth is required.")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "DOB must be in YYYY-MM-DD format.")]
        public string? DOB { get; set; }

        [Required(ErrorMessage = "NIC number is required.")]
        [RegularExpression(@"^(\d{9}[vV]|\d{12})$", ErrorMessage = "NIC must be 9 digits ending with 'V' or 12 digits.")]
        public string? NIC { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(?:0|\+94)?(7\d)\d{7}$", ErrorMessage = "Invalid Sri Lankan mobile number. Must be 10 digits and start with 07 or +947.")]
        public string? MobileNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }
    }
}