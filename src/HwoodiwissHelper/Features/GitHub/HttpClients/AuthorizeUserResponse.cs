﻿using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public sealed class AuthorizeUserResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }
    
    [JsonPropertyName("expires_in")]
    public required long ExpiresIn { get; init; }
    
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }
    
    [JsonPropertyName("refresh_token_expires_in")]
    public required long RefreshTokenExpiresIn { get; init; }
    
    [JsonPropertyName("scope")]
    public required string Scope { get; init; }
    
    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }
}
