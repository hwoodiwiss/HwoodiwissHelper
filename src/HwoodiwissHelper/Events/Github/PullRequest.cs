using System.Text.Json.Serialization;
using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper.Events.Github;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(Opened), "opened")]
public abstract record PullRequest(Actor Sender, Installation Installation) : GithubWebhookEvent(Sender, Installation)
{
    public sealed record Opened(
        [property: JsonPropertyName("number")]
        int Number,
        [property: JsonPropertyName("pull_request")]
        PullRequestInfo PullRequest,
        [property: JsonPropertyName("repository")]
        Repository Repository,
        Actor Sender,
        Installation Installation) : PullRequest(Sender, Installation);
}
