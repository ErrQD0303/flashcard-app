using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FlashCardApp.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StudyMode
{
    [EnumMember(Value = "flashcards")]
    Flashcards,
    [EnumMember(Value = "learn")]
    Learn,
    [EnumMember(Value = "test")]
    Test,
    [EnumMember(Value = "match")]
    Match
}