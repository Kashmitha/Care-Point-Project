using System.ComponentModel.DataAnnotations;

namespace CarePoint.API.Models
{
    public class Specialty : BaseEntity
    {
        [Key]
        public int SpecialtyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SpecialtyName { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Navigation property
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}