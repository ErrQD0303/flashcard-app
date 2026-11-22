using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlashCardApp.Enums;

namespace FlashCardApp.Dtos;

public class StudySessionResponseDto
{
    [Required]
    [Description("The unique identifier of the study session.")]
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [Description("The unique identifier of the flash card set associated with the study session.")]
    public string FlashCardSetId { get; set; } = string.Empty;
    
    [Required]
    [Description("The unique identifier of the user who created the study session.")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Description("The mode of the study session.")]
    public StudyMode Mode { get; set; } = StudyMode.Flashcards;

    [Required]
    [Description("The timestamp when the study session was started.")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Description("The timestamp when the study session ended.")]
    public DateTime EndedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [Description("Indicates whether the study session is currently active.")]
    public bool IsActive { get; set; } = true;
}