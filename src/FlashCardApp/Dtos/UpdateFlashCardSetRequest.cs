using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace FlashCardApp.Dtos;

public class UpdateFlashCardSetRequest
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The ID of the flash card set")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("title")]
    [Description("The title of the flash card set")]
    public string Title { get; set; } = default!;
    
    [JsonPropertyName("description")]
    [Description("The description of the flash card set")]
    public string Description { get; set; } = default!;

    [JsonPropertyName("flash_cards")]
    [Description("The flash cards within the flash card set")]
    public UpdateFlashCardRequest[] FlashCards { get; set; } = [];
}
