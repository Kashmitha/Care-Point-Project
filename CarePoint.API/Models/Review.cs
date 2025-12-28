using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarePoint.API.Models
{
    public class Review : BaseEntity
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        [Required]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public User Patient { get; set; } = null!;

        public int? AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment? Appointment { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}