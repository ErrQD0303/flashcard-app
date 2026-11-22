namespace FlashCardApp.Dtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;

public class LoginByEmailRequest
{
    [Required]
    [JsonPropertyName("email")]
    [EmailAddress]
    [Description("The email address of the user.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("password")]
    [Description("The password of the user.")]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
