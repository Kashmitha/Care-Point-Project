using System.ComponentModel.DataAnnotations;

namespace CarePoint.API.DTOs.Auth
{ 
    // DTO for user registration.
    // This prevents over-posting attacks and controls what data is exposed
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
        public string? Gender { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }

        [Required]
        [RegularExpression("^(Patient|Doctor)$", ErrorMessage = "Role must be Patient or Doctor")]
        public string Role { get; set; } = "Patient";

        // Doctor-specific fields (only required if Role is Doctor)
        public string? LicenseNumber { get; set; }
        public int? SpecialtyId { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Qualification { get; set; }
        public string? Bio { get; set; }
        public decimal? ConsultationFee { get; set; }
    }
}