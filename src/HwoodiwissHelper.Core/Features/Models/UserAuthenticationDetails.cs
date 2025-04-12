using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Core.Features.Models;

public class UserAuthenticationDetails
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }
    
    [JsonPropertyName("expires_at")]
    public required DateTimeOffset ExpiresAt { get; init; }
    
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }
    
    [JsonPropertyName("refresh_token_expires_at")]
    public required DateTimeOffset RefreshTokenExpiresAt { get; init; }
    
    [JsonPropertyName("scope")]
    public required string Scope { get; init; }
    
    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }
}
