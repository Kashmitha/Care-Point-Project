namespace CarePoint.API.Models
{
    // This BaseEntity class that provides common properties for all entities.
    public abstract class BaseEntity
    {
        public DateTime CreatedAt {get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }
}