using System.Text.Json.Serialization;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

public record Repository(
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("full_name")]
    string FullName,
    [property: JsonPropertyName("fork")]
    bool Fork
);
