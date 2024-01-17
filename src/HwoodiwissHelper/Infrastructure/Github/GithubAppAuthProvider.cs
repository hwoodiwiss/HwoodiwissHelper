using System.Security.Cryptography;
using HwoodiwissHelper.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HwoodiwissHelper.Infrastructure.Github;

public sealed class GithubAppAuthProvider : IGithubAppAuthProvider
{
    private readonly TimeProvider _timeProvider;
    private GithubConfiguration _githubConfiguration;
    private TokenWithExpiration<JsonWebToken> _githubJwt;

    public GithubAppAuthProvider(TimeProvider timeProvider, IOptionsMonitor<GithubConfiguration> githubConfiguration)
    {
        _timeProvider = timeProvider;
        _githubConfiguration = githubConfiguration.CurrentValue;
        githubConfiguration.OnChange(value =>
        {
            _githubConfiguration = value;
        });
        _githubJwt = new TokenWithExpiration<JsonWebToken>(timeProvider, token => token.ValidTo.AddSeconds(-30));
    }

    public string GetGithubJwt() =>
        _githubJwt.GetOrRenew(GenerateJwt).EncodedToken;

    private JsonWebToken GenerateJwt()
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
        return new JsonWebToken(jwtText);
    }
}
