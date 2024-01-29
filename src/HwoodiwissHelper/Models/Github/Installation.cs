using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

public sealed record Installation(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("node_id")]
    string NodeId);
