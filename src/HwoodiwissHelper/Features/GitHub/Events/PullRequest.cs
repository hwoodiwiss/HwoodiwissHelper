using System.Text.Json.Serialization;
using HwoodiwissHelper.Features.GitHub.Events.Models;

namespace HwoodiwissHelper.Features.GitHub.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(Opened), "opened")]
[JsonDerivedType(typeof(Synchronize), "synchronize")]
public abstract record PullRequest : GitHubWebhookEvent
{
    public sealed record Opened : PullRequest
    {
        [JsonPropertyName("number")]
        public int Number { get; init; }

        [JsonPropertyName("pull_request")]
        public required PullRequestInfo PullRequest { get; init; }

        [JsonPropertyName("repository")]
        public required Repository Repository { get; init; }
    }

    public sealed record Synchronize : PullRequest
    {
        [JsonPropertyName("number")]
        public int Number { get; init; }

        [JsonPropertyName("pull_request")]
        public required PullRequestInfo PullRequest { get; init; }

        [JsonPropertyName("repository")]
        public required Repository Repository { get; init; }
    }
}
