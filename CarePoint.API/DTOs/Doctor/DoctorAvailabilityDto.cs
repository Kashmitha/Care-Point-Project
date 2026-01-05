using System.ComponentModel.DataAnnotations;

namespace CarePoint.API.DTOs.Doctor
{
    /// <summary>
    /// DTO for doctor availability schedule
    /// Represent when a doctor is available for appointments
    /// </summary>
    public class DoctorAvailabilityDto
    {
        public int AvailabilityId { get; set; }
        public int DoctorId { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Day of weeek must be between 0 (Sunday) and 6 (Saturday)")]
        public int DayOfWeek { get; set; }

        public string DayName { get; set; } = string.Empty;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; }
    }

    public class CreateAvailabilityDto
    {
        [Required]
        [Range(0,6)]
        public int DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}