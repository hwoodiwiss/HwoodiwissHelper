using System.Text.Json.Serialization;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Infrastructure.Github;
using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]

public partial class ApplicationJsonContext : JsonSerializerContext
{
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(WorkflowRun))]
[JsonSerializable(typeof(PullRequest))]
[JsonSerializable(typeof(GithubInstallationAuthenticationResponse))]
[JsonSerializable(typeof(GithubUserInstallationResponse))]
public partial class GithubJsonContext : JsonSerializerContext
{
}
