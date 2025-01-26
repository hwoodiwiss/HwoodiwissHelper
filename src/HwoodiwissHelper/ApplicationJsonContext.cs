using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using HwoodiwissHelper.Features.GitHub.Events;
using HwoodiwissHelper.Features.GitHub.HttpClients;

namespace HwoodiwissHelper;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(WorkflowRun))]
[JsonSerializable(typeof(PullRequest))]
[JsonSerializable(typeof(AuthorizeUserResponse))]
public partial class ApplicationJsonContext : JsonSerializerContext;

