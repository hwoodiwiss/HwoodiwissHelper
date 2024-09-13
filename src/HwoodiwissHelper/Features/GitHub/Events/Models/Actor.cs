using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

public sealed record Actor(
    [property: JsonPropertyName("deleted")]
    bool Deleted,
    [property: JsonPropertyName("email")]
    string? Email,
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("login")]
    string Login,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("type")]
    ActorType Type);
