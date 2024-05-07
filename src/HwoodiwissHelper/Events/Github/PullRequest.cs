using System.Text.Json.Serialization;
using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper.Events.Github;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(Opened), "opened")]
[JsonDerivedType(typeof(Synchronize), "synchronize")]
public abstract record PullRequest : GithubWebhookEvent
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
