using HwoodiwissHelper.Core.Features.Models;
using HwoodiwissHelper.Features.GitHub.HttpClients;

namespace HwoodiwissHelper.Features.GitHub.Extension;

public static class AuthorizeUserResponseExtensions
{
    private readonly static TimeSpan s_minusThreeMinutes = TimeSpan.FromMinutes(-3);

    public static UserAuthenticationDetails ToUserAuthDetails(this AuthorizeUserResponse response) =>
        new()
        {
            AccessToken = response.AccessToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn).Add(s_minusThreeMinutes),
            RefreshToken = response.RefreshToken,
            RefreshTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(response.RefreshTokenExpiresIn).Add(s_minusThreeMinutes),
            Scope = response.Scope,
            TokenType = response.TokenType,
        };

}
