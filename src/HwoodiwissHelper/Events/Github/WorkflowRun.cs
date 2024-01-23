using System.Text.Json.Serialization;
using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper.Events.Github;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(Completed), "completed")]
[JsonDerivedType(typeof(InProgress), "in_progress")]
[JsonDerivedType(typeof(Requested), "requested")]
public abstract record WorkflowRun(Actor Sender, Installation Installation) : GithubWebhookEvent(Sender, Installation)
{
    public sealed record Completed(        
        [property: JsonPropertyName("workflow_run")]
        WorkflowRunInfo WorkflowRun,
        Actor Sender,
        Installation Installation) : WorkflowRun(Sender, Installation);
    public sealed record InProgress(Actor Sender, Installation Installation) : WorkflowRun(Sender, Installation);
    public sealed record Requested(Actor Sender, Installation Installation) : WorkflowRun(Sender, Installation);
}
