using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Events.Github;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(Opened), "opened")]
public abstract record PullRequest() : GithubWebhookEvent
{
    public sealed record Opened() : PullRequest;
}
