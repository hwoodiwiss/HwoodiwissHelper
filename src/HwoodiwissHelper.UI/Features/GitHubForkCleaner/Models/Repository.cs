using System.Text.Json.Serialization;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

public sealed record Repository(
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("full_name")]
    string FullName,
    [property: JsonPropertyName("fork")]
    bool Fork,
    [property: JsonPropertyName("owner")]
    Owner Owner
);
