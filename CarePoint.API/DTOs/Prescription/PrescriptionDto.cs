using System.ComponentModel.DataAnnotations;

namespace CarePoint.API.DTOs.Prescription
{
    public class PrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string? Diagnosis { get; set; } 
        public string? Instructions { get; set; }
        public DateTime CreatedAt { get; set; } 
        public List<MedicationDto> Medications { get; set; } = new List<MedicationDto>();
    }

    public class MedicationDto
    {
        public int MedicationId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
        public string? Notes { get; set; }
    }

    public class CreatePrescriptionDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [MaxLength(1000)]
        public string? Diagnosis { get; set; }

        [MaxLength(2000)]
        public string? Instructions { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one medication is required")]
        public List<CreateMedicationDto> Meications { get; set; } = new List<CreateMedicationDto>(); 
    }

    public class CreateMedicationDto
    {
        [Required]
        [MaxLength(255)]
        public string MedicineName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Dosage { get; set; }

        [MaxLength(100)]
        public string? Frequency { get; set; }

        [MaxLength(100)]
        public string? Duration { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}