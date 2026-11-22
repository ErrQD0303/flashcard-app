using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FlashCardApp.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FlashCardSetVisibility
{
    [EnumMember(Value = "public")]
    Public,
    [EnumMember(Value = "private")]
    Private,
    [EnumMember(Value = "link")]
    Link
}