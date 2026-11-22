namespace FlashCardApp.Dtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;

public class RegisterRequest
{
    [Required]
    [JsonPropertyName("username")]
    [Description("The username of the user.")]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("email")]
    [EmailAddress]
    [Description("The email address of the user.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("password")]
    [MinLength(8)]
    [Description("The password of the user.")]
    public string Password { get; set; } = string.Empty;
}
