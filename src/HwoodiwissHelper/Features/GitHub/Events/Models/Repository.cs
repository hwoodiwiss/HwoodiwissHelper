using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

public record Repository(
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("owner")]
    Actor Owner
    );
