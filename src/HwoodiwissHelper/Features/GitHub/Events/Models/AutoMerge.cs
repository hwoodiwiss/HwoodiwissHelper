using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
public sealed record AutoMerge(
    [property: JsonPropertyName("enabled_by")]
    Actor EnabledBy,
    [property: JsonPropertyName("merge_method")]
    MergeMethod MergeMethod,
    [property: JsonPropertyName("commit_title")]
    string CommitTitle,
    [property: JsonPropertyName("commit_message")]
    string CommitMessage
    );
