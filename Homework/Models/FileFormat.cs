using Json.More;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Homework.Models
{
    [Flags]
    [JsonConverter(typeof(EnumStringConverter<FileFormat>))]
    public enum FileFormat
    {
        [EnumMember(Value = "json")]
        [Description("json")]
        Json = 1,
        [EnumMember(Value = "xml")]
        [Description("xml")]
        Xml = 2,
        [EnumMember(Value = "undefined")]
        [Description("undefined")]
        Undefined = 1000
    }
}
