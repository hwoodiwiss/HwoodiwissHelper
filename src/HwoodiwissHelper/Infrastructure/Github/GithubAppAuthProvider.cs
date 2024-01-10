using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using HwoodiwissHelper.Configuration;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HwoodiwissHelper.Infrastructure.Github;

public sealed class GithubAppAuthProvider : IGithubAppAuthProvider
{
    private readonly TimeProvider _timeProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonTypeInfo<GithubUserInstallationResponse> _userResponseTypeInfo;
    private readonly JsonTypeInfo<GithubInstallationAuthenticationResponse> _tokenResponseTypeInfo;
    private GithubConfiguration _githubConfiguration;
    private GithubInstallationAuthenticationResponse? _token;

    public GithubAppAuthProvider(TimeProvider timeProvider, IOptionsMonitor<GithubConfiguration> githubConfiguration, IHttpClientFactory httpClientFactory, IOptions<JsonOptions> jsonOptions)
    {
        _timeProvider = timeProvider;
        _httpClientFactory = httpClientFactory;
        _userResponseTypeInfo = GetJsonTypeInfo<GithubUserInstallationResponse>(jsonOptions.Value.SerializerOptions);
        _tokenResponseTypeInfo = GetJsonTypeInfo<GithubInstallationAuthenticationResponse>(jsonOptions.Value.SerializerOptions);
        _githubConfiguration = githubConfiguration.CurrentValue;
        githubConfiguration.OnChange(value =>
        {
            _githubConfiguration = value;
        });
    }
    
    public async Task<string> GetToken()
    {
        if (_token is not null && _timeProvider.GetUtcNow() <= _token.ExpiresAt.AddSeconds(-30))
        {
            return _token.Token;
        }
        
        var jwt = GenerateJwt();
        using var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://api.github.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HwoodiwissHelper", "1.0"));
        var getInstallationResponse = await client.GetAsync($"/users/hwoodiwiss/installation");
        var installation = await getInstallationResponse.Content.ReadFromJsonAsync(_userResponseTypeInfo);
        var getAccessTokenResponse = await client.PostAsync($"/app/installations/{installation?.Id}/access_tokens", null);
        _token = await getAccessTokenResponse.Content.ReadFromJsonAsync(_tokenResponseTypeInfo);
        return _token?.Token ?? string.Empty;
    }

    private string GenerateJwt()
    {
        var tokenHandler = new JsonWebTokenHandler();
        var tokenDesc = new SecurityTokenDescriptor
        {
            Issuer = _githubConfiguration.AppId,
            Expires = _timeProvider.GetUtcNow().AddMinutes(9).DateTime
        };
        using var rsa = RSA.Create();
        rsa.ImportFromPem(_githubConfiguration.AppPrivateKey);
        tokenDesc.SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        var jwtText = tokenHandler.CreateToken(tokenDesc);
        return jwtText;
    }

    private static JsonTypeInfo<T> GetJsonTypeInfo<T>(JsonSerializerOptions options)
    {
        if (options.TryGetTypeInfo(typeof(T), out var userResponseTypeInfo) &&
            userResponseTypeInfo is JsonTypeInfo<T> info)
        {
            return info;
        }

        throw new ArgumentException($"Unable to find JsonTypeInfo for {typeof(T).FullName}");
    }
}
