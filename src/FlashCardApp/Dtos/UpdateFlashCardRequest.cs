using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace FlashCardApp.Dtos;

public class UpdateFlashCardRequest
{
    [JsonPropertyName("term")]
    [Description("The term of the flash card")]
    public string Term { get; set; } = default!;

    [JsonPropertyName("definition")]
    [Description("The definition of the flash card")]
    public string Definition { get; set; } = default!;

    [JsonPropertyName("position")]
    [Description("The position of the flash card within the set")]
    public int Position { get; set; } = default!;
}