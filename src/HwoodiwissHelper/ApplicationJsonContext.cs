using System.Text.Json.Serialization;

using HwoodiwissHelper.Features;
using HwoodiwissHelper.Features.Configuration;

namespace HwoodiwissHelper;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
[JsonSerializable(typeof(VersionConfiguration))]
public partial class ApplicationJsonContext : JsonSerializerContext
{
}
