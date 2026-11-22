using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FlashCardApp.Enums;

namespace FlashCardApp.Dtos;

public class FlashCardSetResponseDto
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The unique identifier of the flashcard set")]
    public string Id { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("owner_id")]
    [Description("The unique identifier of the owner of the flashcard set")]
    public string OwnerId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("title")]
    [Description("The title of the flashcard set")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("description")]
    [Description("The description of the flashcard set")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("visibility")]
    [Description("The visibility of the flashcard set")]
    public FlashCardSetVisibility Visibility { get; set; } = FlashCardSetVisibility.Public;

    [Required]
    [JsonPropertyName("flashcards")]
    [Description("The list of flashcards in the flashcard set")]
    public List<FlashCardResponseDto> FlashCards { get; set; } = [];
}
