using FlashCardApp.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace FlashCardApp.Dtos;

public class CardReviewResponseDto
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The unique identifier of the card review.")]
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [JsonPropertyName("study_session_id")]
    [Description("The unique identifier of the review session.")]
    public string SessionId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("card_id")]
    [Description("The unique identifier of the card.")]
    public string CardId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("result")]
    [Description("The result of the card review.")]
    // Convert the enum value to its string representation in JSON
    public ReviewResult Result { get; set; } = ReviewResult.Skipped;

    [Required]
    [JsonPropertyName("response_time_ms")]
    [Description("The response time of the review in milliseconds.")]
    public int ResponseTimeMs { get; set; } = 0;

    [Required]
    [JsonPropertyName("reviewed_at")]
    [Description("The timestamp when the card was reviewed.")]
    public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
}