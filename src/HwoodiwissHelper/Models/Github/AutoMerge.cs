using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
public sealed record AutoMerge(
    Actor EnabledBy,
    MergeMethod MergeMethod,
    string CommitTitle,
    string CommitMessage
    );
