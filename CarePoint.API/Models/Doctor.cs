using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarePoint.API.Models
{
    public class Doctor : BaseEntity
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!; // Explicit relationship

        public int? SpecialtyId { get; set; }

        [ForeignKey(nameof(SpecialtyId))]
        public Specialty? Specialty { get; set; } // Explicit relationship

        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        public int? YearsOfExperience { get; set; }

        [MaxLength(255)]
        public string? Qualification { get; set; }

        public string? Bio { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? ConsultationFee { get; set; }

        public bool IsApproved { get; set; } = false;

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        // Nav props
        public ICollection Availabilities { get; set; } = new List();
        public ICollection Appointments { get; set; } = new List();
        public ICollection Prescriptions { get; set; } = new List();
        public ICollection Reviews { get; set; } = new List();
    }
}