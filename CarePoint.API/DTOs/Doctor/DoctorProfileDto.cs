using System.ComponentModel.DataAnnotations;

namespace CarePoint.API.DTOs.Doctor
{
    /// <summary>
    /// DTO for doctor profile information.
    /// </summary>
    public class DoctorProfileDto
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? SpecialtyName { get; set; }
        public int? SpecialtyId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public string? Qualification { get; set; }
        public string? Bio { get; set; }
        public decimal? ConsultationFee { get; set; }
        public bool IsApproved { get; set; }
        public double? AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }

    /// <summary>
    /// DTO for updating doctor profile.
    /// </summary>
    public class UpdateDoctorProfileDto
    {
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [MaxLength(500)]
        public string? Address { get; set; }
        
        [MaxLength(100)]
        public string? City { get; set; }
        
        [MaxLength(100)]
        public string? State { get; set; }
        
        [MaxLength(20)]
        public string? ZipCode { get; set; }
        
        [MaxLength(2000)]
        public string? Bio { get; set; }
        
        [Range(0, 10000)]
        public decimal? ConsultationFee { get; set; }
        
        [Range(0, 100)]
        public int? YearsOfExperience { get; set; }
        
        [MaxLength(255)]
        public string? Qualification { get; set; }
    }
}