using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

public sealed record Label(
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("url")]
    string Url,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("description")]
    string Description,
    [property: JsonPropertyName("color")]
    string Colour,
    [property: JsonPropertyName("default")]
    bool Default);
