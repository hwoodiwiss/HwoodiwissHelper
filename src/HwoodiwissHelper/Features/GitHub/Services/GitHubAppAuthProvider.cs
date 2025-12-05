using System.Security.Cryptography;
using HwoodiwissHelper.Features.GitHub.Configuration;
using HwoodiwissHelper.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HwoodiwissHelper.Features.GitHub.Services;

public sealed class GitHubAppAuthProvider(TimeProvider timeProvider, IOptionsMonitor<GitHubConfiguration> githubConfiguration) : IGitHubAppAuthProvider
{
    private TokenWithExpiration<JsonWebToken> _githubJwt = new(timeProvider, token => token.ValidTo.AddSeconds(-30));

    public string GetGithubJwt() =>
        _githubJwt.GetOrRenew(GenerateJwt).EncodedToken;

    private JsonWebToken GenerateJwt()
    {
        var tokenHandler = new JsonWebTokenHandler();
        var tokenDesc = new SecurityTokenDescriptor
        {
            Issuer = githubConfiguration.CurrentValue.AppId,
            Expires = timeProvider.GetUtcNow().AddMinutes(9).DateTime
        };
        using var rsa = RSA.Create();
        rsa.ImportFromPem(githubConfiguration.CurrentValue.AppPrivateKey);
        tokenDesc.SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            // This is required to prevent the signature provider from being cached, causing an ObjectDisposedException
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };
        var jwtText = tokenHandler.CreateToken(tokenDesc);
        return new JsonWebToken(jwtText);
    }
}
