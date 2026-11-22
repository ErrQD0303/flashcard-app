using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FlashCardApp.Enums;

namespace FlashCardApp.Dtos;

public class CreatedStudySessionResponse
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The unique identifier of the created study session.")]
    public string Id { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("set_id")]
    [Description("The unique identifier of the flash card set associated with the study session.")]
    public string FlashCardSetId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("user_id")]
    [Description("The unique identifier of the user who created the study session.")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("mode")]
    [Description("The mode of the study session.")]
    public StudyMode Mode { get; set; } = StudyMode.Flashcards;

    [Required]
    [JsonPropertyName("started_at")]
    [Description("The timestamp when the study session was started.")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [JsonPropertyName("is_active")]
    [Description("Indicates whether the study session is currently active.")]
    public bool IsActive { get; set; }

}