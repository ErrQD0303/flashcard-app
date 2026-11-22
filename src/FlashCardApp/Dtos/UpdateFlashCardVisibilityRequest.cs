using FlashCardApp.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace FlashCardApp.Dtos;

public class UpdateFlashCardSetVisibilityRequest
{
    [Required]
    [JsonPropertyName("visibility")]
    [Description("The visibility of the flash card set.")]
    public FlashCardSetVisibility Visibility { get; set; } = default!;
}