using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public record InstallationTokenRequest
{
    [JsonPropertyName("repositories")]
    public string[]? Repositories { get; init; }

    [JsonPropertyName("permissions")]
    public Dictionary<InstallationScope, InstallationOperation> Permissions { get; init; } = new();
}

[JsonConverter(typeof(JsonStringEnumConverter<InstallationScope>))]
public enum InstallationScope
{
    [JsonStringEnumMemberName("pull_requests")]
    PullRequests,
}


[JsonConverter(typeof(JsonStringEnumConverter<InstallationOperation>))]
public enum InstallationOperation
{
    [JsonStringEnumMemberName("write")]
    Write,
}
