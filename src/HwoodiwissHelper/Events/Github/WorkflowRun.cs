using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Events.Github;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(Complete), "complete")]
[JsonDerivedType(typeof(InProgress), "in_progress")]
[JsonDerivedType(typeof(Requested), "requested")]
public abstract record WorkflowRun() : GithubWebhookEvent
{
    public sealed record Complete() : WorkflowRun;
    public sealed record InProgress() : WorkflowRun;
    public sealed record Requested() : WorkflowRun;
}
