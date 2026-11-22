using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FlashCardApp.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReviewResult
{
    [EnumMember(Value = "correct")]
    Correct,
    [EnumMember(Value = "incorrect")]
    Incorrect,
    [EnumMember(Value = "skipped")]
    Skipped
}