using System.Text.Json.Serialization;
using HwoodiwissHelper.Events.Github;

namespace HwoodiwissHelper;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(WorkflowRun))]
[JsonSerializable(typeof(PullRequest))]
public partial class ApplicationJsonContext : JsonSerializerContext
{
}
