using System.Text.Json.Serialization;
using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper.Events.Github;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "action")]
[JsonDerivedType(typeof(Completed), "completed")]
[JsonDerivedType(typeof(InProgress), "in_progress")]
[JsonDerivedType(typeof(Requested), "requested")]
public abstract record WorkflowRun(Actor Sender) : GithubWebhookEvent(Sender)
{ 
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    public sealed record Completed(WorkflowRunInfo WorkflowRun, Actor Sender) : WorkflowRun(Sender);
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    public sealed record InProgress(Actor Sender) : WorkflowRun(Sender);
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    public sealed record Requested(Actor Sender) : WorkflowRun(Sender);
}
