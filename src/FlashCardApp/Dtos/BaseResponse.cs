namespace FlashCardApp.Dtos;
using System.Text.Json.Serialization;
using System.ComponentModel;
public class BaseResponse
{
    [JsonPropertyName("success")]
    [Description("Indicates whether the operation was successful")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    [Description("The message associated with the operation result")]
    public string? Message { get; set; }
}
