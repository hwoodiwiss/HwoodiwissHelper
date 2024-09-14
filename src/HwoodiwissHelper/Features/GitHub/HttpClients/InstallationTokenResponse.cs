using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public class InstallationTokenResponse
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }

    [JsonPropertyName("expires_at")]
    public required DateTimeOffset ExpiresAt { get; init; }
}
