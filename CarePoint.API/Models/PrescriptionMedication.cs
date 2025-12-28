using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarePoint.API.Models
{
    public class PrescriptionMedication
    {
        [Key]
        public int MedicationId { get; set; }

        [Required]
        public int PrescriptionId { get; set; }

        [ForeignKey(nameof(PrescriptionId))]
        public Prescription Prescription { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string MedicineName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Dosage { get; set; }

        [MaxLength(100)]
        public string? Frequency { get; set; }

        [MaxLength(100)]
        public string? Duration { get; set; }

        public string? Notes { get; set; }
    }
}