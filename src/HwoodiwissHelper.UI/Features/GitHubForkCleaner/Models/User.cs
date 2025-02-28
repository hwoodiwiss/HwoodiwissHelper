using System.Text.Json.Serialization;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

public sealed record User(
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("login")]
    string Login
);
