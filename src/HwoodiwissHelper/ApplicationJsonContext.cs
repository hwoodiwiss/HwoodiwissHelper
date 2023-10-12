using System.Text.Json.Serialization;
using HwoodiwissHelper.Endpoints;
using HwoodiwissHelper.Events.Github;

namespace HwoodiwissHelper;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(WorkflowRun))]
public partial class ApplicationJsonContext : JsonSerializerContext
{
}
