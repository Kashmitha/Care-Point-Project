namespace CarePoint.API.DTOs.Doctor
{
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
}