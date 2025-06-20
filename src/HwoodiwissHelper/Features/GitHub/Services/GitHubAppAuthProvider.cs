using System.Security.Cryptography;
using HwoodiwissHelper.Features.GitHub.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HwoodiwissHelper.Features.GitHub.Services;

public sealed class GitHubAppAuthProvider(TimeProvider timeProvider, IMemoryCache tokenCache, IOptionsMonitor<GitHubConfiguration> githubConfiguration) : IGitHubAppAuthProvider
{
    public string GetGithubJwt(string appId) =>
        tokenCache.GetOrCreate(appId, (item) =>
        {
            JsonWebToken jwt =  GenerateJwt(appId);
            item.AbsoluteExpiration = jwt.ValidTo.AddSeconds(-30);
            return jwt;
        })!.EncodedToken;
    
    private JsonWebToken GenerateJwt(string appId)
    {
        var appConfig = githubConfiguration.CurrentValue.AppConfigurations.Values
            .First(w => w.AppId == appId);
        
        var tokenHandler = new JsonWebTokenHandler();
        var tokenDesc = new SecurityTokenDescriptor
        {
            Issuer = appConfig.AppId,
            Expires = timeProvider.GetUtcNow().AddMinutes(9).DateTime
        };
        using var rsa = RSA.Create();
        rsa.ImportFromPem(appConfig.PrivateKey);
        tokenDesc.SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            // This is required to prevent the signature provider from being cached, causing an ObjectDisposedException
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };
        var jwtText = tokenHandler.CreateToken(tokenDesc);
        return new JsonWebToken(jwtText);
    }
}
