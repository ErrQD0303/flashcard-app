namespace FlashCardApp.Dtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;
public class LoginByUsernameRequest
{
    [Required]
    [JsonPropertyName("username")]
    [MinLength(3)]
    [Description("The username of the user.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("password")]
    [MinLength(8)]
    [Description("The password of the user.")]
    public string Password { get; set; } = string.Empty;
}
