using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using HwoodiwissHelper.Configuration;
using HwoodiwissHelper.Extensions;
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
    private TokenWithExpiration<JsonWebToken> _githubJwt;
    private TokenWithExpiration<GithubInstallationAuthenticationResponse?> _token;

    public GithubAppAuthProvider(TimeProvider timeProvider, IOptionsMonitor<GithubConfiguration> githubConfiguration, IHttpClientFactory httpClientFactory, IOptions<JsonOptions> jsonOptions)
    {
        _timeProvider = timeProvider;
        _httpClientFactory = httpClientFactory;
        _userResponseTypeInfo = jsonOptions.Value.SerializerOptions.GetJsonTypeInfo<GithubUserInstallationResponse>();
        _tokenResponseTypeInfo = jsonOptions.Value.SerializerOptions.GetJsonTypeInfo<GithubInstallationAuthenticationResponse>();
        _githubConfiguration = githubConfiguration.CurrentValue;
        githubConfiguration.OnChange(value =>
        {
            _githubConfiguration = value;
        });
        _githubJwt = new TokenWithExpiration<JsonWebToken>(timeProvider, token => token.ValidTo.AddSeconds(-30));
        _token = new TokenWithExpiration<GithubInstallationAuthenticationResponse?>(timeProvider, token => token?.ExpiresAt.AddSeconds(-30).UtcDateTime ?? DateTime.MinValue);
    }

    public async ValueTask<string> GetGithubJwt() =>
        (await _githubJwt.GetOrRenewAsync(GenerateJwt)).EncodedToken;

    public async ValueTask<string?> GetToken() =>
        (await _token.GetOrRenewAsync(GenerateAccessToken))?.Token;

    private ValueTask<JsonWebToken> GenerateJwt()
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
        return new ValueTask<JsonWebToken>(new JsonWebToken(jwtText));
    }

    private async ValueTask<GithubInstallationAuthenticationResponse?> GenerateAccessToken()
    {
        var jwt = await GetGithubJwt();
        using var client = _httpClientFactory.CreateClient("Github");
        var installation = await GetUserInstallation(client, jwt);
        var getAccessTokenResponse = await client.PostAsync($"/app/installations/{installation?.Id}/access_tokens", null);
        return await getAccessTokenResponse.Content.ReadFromJsonAsync(_tokenResponseTypeInfo);
    }

    private async Task<GithubUserInstallationResponse?> GetUserInstallation(HttpClient client, string jwt)
    {
        using HttpRequestMessage requestMessage = new(HttpMethod.Get, "/users/hwoodiwiss/installation");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var response = await client.SendAsync(requestMessage);
        return await response.Content.ReadFromJsonAsync(_userResponseTypeInfo);
    }
}
