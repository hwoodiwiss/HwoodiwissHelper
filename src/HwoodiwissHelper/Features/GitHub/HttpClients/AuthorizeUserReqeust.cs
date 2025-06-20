using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public sealed class AuthorizeUserRequest : ClientCredentialsRequest
{
    [JsonPropertyName("code")]
    public required string Code { get; init; }

    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; init; }
}
