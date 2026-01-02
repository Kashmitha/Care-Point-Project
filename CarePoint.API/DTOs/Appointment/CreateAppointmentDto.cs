using System.ComponentModel.DataAnnotations;

namespace CarePoint.API.DTOs.Appointment
{
    public class CreateAppointmentDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [MaxLength(500)]
        public string? ReasonForVisit { get; set; }
    }
}