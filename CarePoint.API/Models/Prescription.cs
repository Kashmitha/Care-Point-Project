using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarePoint.API.Models
{
    public class Prescription : BaseEntity
    {
        [Key]
        public int PrescriptionId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment { get; set; } = null!;

        [Required]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public User Patient { get; set; } = null!;

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        public string? Diagnosis { get; set; }

        public string? Instructions { get; set; }

        // Navigation property
        public ICollection<PrescriptionMedication> Medications { get; set; } = new List<PrescriptionMedication>();
    }
}