using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

public sealed record Branch(
    [property: JsonPropertyName("label")]
    string Label,
    [property: JsonPropertyName("ref")]
    string Ref,
    [property: JsonPropertyName("sha")]
    string Sha,
    [property: JsonPropertyName("user")]
    Actor User);
