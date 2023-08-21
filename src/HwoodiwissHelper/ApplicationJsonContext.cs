using System.Text.Json.Serialization;

namespace HwoodiwissHelper;

[JsonSerializable(typeof(object))]
public partial class ApplicationJsonContext : JsonSerializerContext
{
}
