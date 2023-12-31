﻿using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
public sealed record WorkflowRunInfo(
    Actor Actor,
    string ArtifactsUrl,
    string CancelUrl,
    int CheckSuiteId,
    string CheckSuiteNodeId,
    string CheckSuiteUrl,
    WorkflowConclusion? Conclusion,
    string CreatedAt,
    string Event,
    string? HeadBranch,
    string HeadSha,
    string HtmlUrl,
    int Id,
    string JobsUrl,
    string LogsUrl,
    string? Name,
    string NodeId,
    string Path,
    string? PreviousAttemptUrl,
    string RerunUrl,
    int RunAttempt,
    int RunNumber,
    string RunStartedAt,
    WorkflowStatus Status,
    Actor? TriggeringActor,
    string UpdatedAt,
    string Url,
    int WorkflowId,
    string WorkflowUrl);
