using System.Text.Json.Serialization;

namespace HwoodiwissHelper;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
public partial class ApplicationJsonContext : JsonSerializerContext
{
}
