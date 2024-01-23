using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

public sealed record WorkflowRunInfo(
    [property: JsonPropertyName("actor")]
    Actor Actor,
    [property: JsonPropertyName("check_suite_id")]
    long CheckSuiteId,
    [property: JsonPropertyName("conclusion")]
    WorkflowConclusion? Conclusion,
    [property: JsonPropertyName("created_at")]
    string CreatedAt,
    [property: JsonPropertyName("event")]
    string Event,
    [property: JsonPropertyName("head_branch")]
    string? HeadBranch,
    [property: JsonPropertyName("head_sha")]
    string HeadSha,
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("name")]
    string? Name,
    [property: JsonPropertyName("run_attempt")]
    int RunAttempt,
    [property: JsonPropertyName("run_number")]
    int RunNumber,
    [property: JsonPropertyName("run_started_at")]
    string RunStartedAt,
    [property: JsonPropertyName("status")]
    WorkflowStatus Status,
    [property: JsonPropertyName("triggering_actor")]
    Actor? TriggeringActor,
    [property: JsonPropertyName("updated_at")]
    string UpdatedAt,
    [property: JsonPropertyName("workflow_id")]
    int WorkflowId);
