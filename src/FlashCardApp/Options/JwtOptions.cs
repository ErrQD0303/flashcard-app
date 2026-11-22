using System.ComponentModel.DataAnnotations;
using FlashCardApp.Utils;

namespace FlashCardApp.Options;

public class JwtOptions
{
    [Required]
    public string Authority { get; set; } = default!;

    [Required]
    public string Audience { get; set; } = default!;

    [Required]
    [MinLength(32, ErrorMessage = $"{AppConstants.App_Options.JwtOptionsSection}:{nameof(SecretKey)} must be at least 32 characters long.")]
    public string SecretKey { get; set; } = default!;
}