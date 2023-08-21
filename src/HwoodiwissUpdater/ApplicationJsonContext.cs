using System.Text.Json.Serialization;

namespace HwoodiwissUpdater;

[JsonSerializable(typeof(object))]
public partial class ApplicationJsonContext : JsonSerializerContext
{
}
