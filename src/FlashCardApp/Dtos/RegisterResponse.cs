namespace FlashCardApp.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using System.Text.Json.Serialization;

public class RegisterResponse
{
    [Required]
    [JsonPropertyName("user_id")]
    [Description("The unique identifier of the user.")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("username")]
    [Description("The username of the user.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("email")]
    [Description("The email address of the user.")]
    public string Email { get; set; } = string.Empty;
}