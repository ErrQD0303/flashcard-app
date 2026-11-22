using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlashCardApp.Dtos;

public class FlashCardResponseDto
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The unique identifier of the flashcard")]
    public string Id { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("set_id")]
    [Description("The unique identifier of the flashcard set this flashcard belongs to")]
    public string FlashCardSetId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("term")]
    [Description("The term of the flashcard")]
    public string Term { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("definition")]
    [Description("The definition of the flashcard")]
    public string Definition { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("position")]
    [Description("The position of the flashcard within the set")]
    public int Position { get; set; } = 0;
}