namespace FlashCardApp.Dtos;
using System.Text.Json.Serialization;
using System.ComponentModel;
public class DataResponse<T> : BaseResponse
{
    [JsonPropertyName("data")]
    [Description("The data associated with the response")]
    public T? Data { get; set; }

    public static DataResponse<T> Ok(T data, string? message = null)
    {
        return new DataResponse<T>
        {
            Data = data,
            Success = true,
            Message = message
        };
    }

    public static DataResponse<T> Fail(string errorMessage, T? data = default)
    {
        return new DataResponse<T>
        {
            Success = false,
            Message = errorMessage,
            Data = data
        };
    }
}