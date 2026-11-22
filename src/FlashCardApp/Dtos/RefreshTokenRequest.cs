namespace FlashCardApp.Dtos;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;

public class RefreshTokenRequest
{
    [Required]
    [JsonPropertyName("token")]
    [Description("The refresh token.")]
    public string Token { get; set; } = string.Empty;
}