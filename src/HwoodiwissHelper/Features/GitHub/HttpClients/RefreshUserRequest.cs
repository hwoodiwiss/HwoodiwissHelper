using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public sealed class RefreshUserRequest : ClientCredentialsRequest
{
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }

    [JsonPropertyName("grant_type")]
    public string GrantType { get; } = "refresh_token";
}
