namespace CarePoint.API.DTOs.Admin
{
    /// <summary>
    /// DTO for admin dashboard statistics
    /// Show overview of system metrics
    /// </summary>
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int ApprovedDoctors { get; set; }
        public int PendingDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public List<SpecialtyStatsDto> SpecialtyStats { get; set; } = new List<SpecialtyStatsDto>();
    }

    public class SpecialtyStatsDto
    {
        public string SpecialtyName { get; set; } = string.Empty;
        public int DoctorCount { get; set; }
        public int AppointmentCount { get; set; }
    }

    public class DoctorApprovalDto
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string? SpecialtyName { get; set; } 
        public int? YearsOfExperience { get; set; }
        public string? Qualification { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}