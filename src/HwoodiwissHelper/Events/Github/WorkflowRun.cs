using System.Text.Json.Serialization;
using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper.Events.Github;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(Completed), "completed")]
[JsonDerivedType(typeof(InProgress), "in_progress")]
[JsonDerivedType(typeof(Requested), "requested")]
public abstract record WorkflowRun : GithubWebhookEvent
{
    public sealed record Completed : WorkflowRun
    {
        [JsonPropertyName("workflow_run")]
        public required WorkflowRunInfo WorkflowRun { get; init; }
    }

    public sealed record InProgress : WorkflowRun;
    public sealed record Requested : WorkflowRun;
}
