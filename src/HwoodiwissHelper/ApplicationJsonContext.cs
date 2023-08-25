using System.Text.Json.Serialization;

namespace HwoodiwissHelper;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
public partial class ApplicationJsonContext : JsonSerializerContext
{
}
