using System.Text.Json.Serialization;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

public sealed record Owner(
    [property: JsonPropertyName("login")]
    string Login
);
