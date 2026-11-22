namespace FlashCardApp.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using System.Text.Json.Serialization;

public class TokenResponse
{
    [Required]
    [JsonPropertyName("access_token")]
    [Description("The access token.")]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("refresh_token")]
    [Description("The refresh token.")]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("token_type")]
    [Description("The scheme of the token.")]
    public string TokenType { get; set; } = "Bearer";
}
