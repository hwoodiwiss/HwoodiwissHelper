using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public sealed class RefreshUserRequest
{
    [JsonPropertyName("client_id")]
    public required string ClientId { get; init; }

    [JsonPropertyName("client_secret")]
    public required string ClientSecret { get; init; }

    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }

    [JsonPropertyName("grant_type")]
    public string GrantType { get; } = "refresh_token";
}
