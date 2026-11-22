namespace FlashCardApp.Dtos;
using System.Text.Json.Serialization;
using System.ComponentModel;    
public class UpdateFlashCardPositionRequest
{
    [JsonPropertyName("position")]
    [Description("The new position of the flash card")]
    public int Position { get; set; } = default!;
}