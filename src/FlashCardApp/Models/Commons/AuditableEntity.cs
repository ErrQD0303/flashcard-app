namespace FlashCardApp.Models.Commons;
using System.ComponentModel.DataAnnotations;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

public abstract class Entity<T>
{
    public required T Id { get; set; }
}

public abstract class AuditableEntity<T> : 
Entity<T>, IAuditableEntity
{
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }

}