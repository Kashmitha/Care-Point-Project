using System.ComponentModel.Annotations;

namespace CarePoint.API.DTOs.Appointment
{
    public class UpddateAppointmentDto
    {
        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [MaxLength(500)]
        public string? ReasonForVisit { get; set; }
    }

    public class CancelAppointmentDto
    {
        [Required]
        [MaxLength(500)]
        public string CancellationReason { get; set; } = string.Empty;
    }

    public class CompleteAppointmentDto
    {
        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}