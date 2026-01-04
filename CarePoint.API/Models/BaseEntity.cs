namespace CarePoint.API.Models
{
    /// <summary>
    /// Base entity class that provides common properties for all entities.
    /// INTERVIEW TIP: Explain benefits of base entities (DRY principle, audit tracking)
    /// </summary>
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}