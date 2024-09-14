using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

public sealed record Branch(
    [property: JsonPropertyName("label")]
    string Label,
    [property: JsonPropertyName("ref")]
    string Ref,
    [property: JsonPropertyName("sha")]
    string Sha);
