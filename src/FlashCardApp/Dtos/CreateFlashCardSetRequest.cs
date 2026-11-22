namespace FlashCardApp.Dtos;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
public class CreateFlashCardSetRequest
{
    [Required]
    [JsonPropertyName("title")]
    [Description("The title of the flash card set")]
    public string Title { get; set; } = default!;

    [Required]
    [JsonPropertyName("description")]
    [Description("The description of the flash card set")]
    public string Description { get; set; } = default!;

    [Required]
    [JsonPropertyName("visibility")]
    [Description("The visibility of the flash card set")]
    public string Visibility { get; set; } = default!;
    [JsonPropertyName("source_set_id")]
    [Description("The ID of the source flash card set")]
    public string SourceSetId { get; set; } = default!;

    [JsonPropertyName("flashcards")]
    [Description("The flash cards within the flash card set")]
    public CreateFlashCardRequest[] FlashCards { get; set; } = [];
}
