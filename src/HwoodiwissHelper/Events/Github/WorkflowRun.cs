using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Events.Github;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(Completed), "completed")]
[JsonDerivedType(typeof(InProgress), "in_progress")]
[JsonDerivedType(typeof(Requested), "requested")]
public abstract record WorkflowRun() : GithubWebhookEvent
{
    public sealed record Completed() : WorkflowRun;
    public sealed record InProgress() : WorkflowRun;
    public sealed record Requested() : WorkflowRun;
}
