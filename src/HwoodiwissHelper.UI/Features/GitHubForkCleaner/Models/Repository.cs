using System.Text.Json.Serialization;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

public record Repository(
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("fork")]
    bool Fork
);
