namespace FlashCardApp.Dtos;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class CreatedFlashCardResponse
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The ID of the created flash card")]
    public string Id { get; set; } = default!;

    [Required]
    [JsonPropertyName("set_id")]
    [Description("The ID of the flash card set this flash card belongs to")]
    public string SetId { get; set; } = default!;

    [Required]
    [JsonPropertyName("term")]
    [Description("The term of the flash card")]
    public string Term { get; set; } = default!;

    [Required]
    [JsonPropertyName("definition")]
    [Description("The definition of the flash card")]
    public string Definition { get; set; } = default!;

    [JsonPropertyName("position")]
    [Description("The position of the flash card within the set")]
    public int Position { get; set; } = default!;
}