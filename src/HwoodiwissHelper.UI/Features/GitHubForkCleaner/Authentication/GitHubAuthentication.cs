using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using HwoodiwissHelper.Core.Features.Models;
using HwoodiwissHelper.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.WebUtilities;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Authentication;

public sealed partial class GitHubAuthentication(
    ICookieManager cookieManager,
    IWebAssemblyHostEnvironment hostEnvironment,
    NavigationManager navigationManager,
    TimeProvider timeProvider,
    ILogger<GitHubAuthentication> logger)
{
    public const string GitHubAuthCookieName = "github_auth";
    private UserAuthenticationDetails? _gitHubAuthDetails;

    public async Task<string> GetAccessToken()
    {
        await EnsureAuthDetails(true);

        return _gitHubAuthDetails!.AccessToken;
    }

    public async Task<bool> IsAuthenticated()
    {
        await EnsureAuthDetails(false);

        return _gitHubAuthDetails is not null;
    }

    [DoesNotReturn]
    public void RedirectToLogin()
    {
        navigationManager.NavigateTo(CalculateLoginUri(), true);

        // HACK: This is a workaround to prevent the compiler from complaining about the method returning.
        // navigationManager.NavigateTo uses a NavigationException internally which means it doesn't return,
        // but it's not marked as such.
        // See: https://github.com/dotnet/aspnetcore/issues/59451
        throw new UnreachableException("This exception should never be thrown. It is a workaround for the compiler.");
    }

    [DoesNotReturn]
    public void RedirectToRefresh()
    {
        navigationManager.NavigateTo(CalculateRefreshUri(), true);

        // HACK: This is a workaround to prevent the compiler from complaining about the method returning.
        // navigationManager.NavigateTo uses a NavigationException internally which means it doesn't return,
        // but it's not marked as such.
        // See: https://github.com/dotnet/aspnetcore/issues/59451
        throw new UnreachableException("This exception should never be thrown. It is a workaround for the compiler.");
    }

    private async Task EnsureAuthDetails(bool redirectToLogin)
    {
        if (_gitHubAuthDetails is not null)
        {
            if (_gitHubAuthDetails.ExpiresAt < timeProvider.GetUtcNow().AddMinutes(-2))
            {
                RedirectToRefresh();
            }

            return;
        }

        var authCookieBase64 = await cookieManager.GetCookieAsync(GitHubAuthCookieName);
        if (string.IsNullOrEmpty(authCookieBase64))
        {
            goto login_failed;
        }

        var authCookieValue = Convert.FromBase64String(authCookieBase64);
        Log.LogGitHubAuthenticationCookie(logger, Encoding.UTF8.GetString(authCookieValue));
        UserAuthenticationDetails? authDetails = JsonSerializer.Deserialize(authCookieValue, GitHubJsonSerializerContext.Default.UserAuthenticationDetails);
        if (authDetails is null)
        {
            goto login_failed;
        }

        _gitHubAuthDetails = authDetails;
        return;

    login_failed:
        if (redirectToLogin)
        {
            RedirectToLogin();
        }
    }


    private string CalculateLoginUri()
    {
        var callbackUriQuery = new Dictionary<string, string?>();
        var loginUriQuery = new Dictionary<string, string?>();
        string? loginUriBase;

        if (hostEnvironment.IsDevelopment())
        {
            loginUriBase = "http://localhost:8080";
            callbackUriQuery.Add("redirectUri", "http://localhost:5238?pageLink=fork-cleaner");
        }
        else
        {
            loginUriBase = hostEnvironment.BaseAddress;
            callbackUriQuery.Add("redirectUri", $"{hostEnvironment.BaseAddress}?pageLink=fork-cleaner");
        }

        var loginRedirectUriWithPath = new UriBuilder(loginUriBase)
        {
            Path = "/github/auth/login/callback"
        };

        loginUriQuery.Add("redirectUri", QueryHelpers.AddQueryString(loginRedirectUriWithPath.ToString(), callbackUriQuery));

        var loginUriWithPath = new UriBuilder(loginUriBase)
        {
            Path = "/github/auth/login"
        };

        var loginUriBuilder = QueryHelpers.AddQueryString(loginUriWithPath.ToString(), loginUriQuery);

        return loginUriBuilder;
    }

    private string CalculateRefreshUri()
    {
        var refreshUriQuery = new Dictionary<string, string?>();
        string? refreshUriBase;

        if (hostEnvironment.IsDevelopment())
        {
            refreshUriBase = "http://localhost:8080";
            refreshUriQuery.Add("redirectUri", "http://localhost:5238?pageLink=fork-cleaner");
        }
        else
        {
            refreshUriBase = hostEnvironment.BaseAddress;
            refreshUriQuery.Add("redirectUri", $"{refreshUriBase}?pageLink=fork-cleaner");
        }

        var refreshUriWithPath = new UriBuilder(refreshUriBase)
        {
            Path = "/github/auth/refresh",
        };

        var refreshUri = QueryHelpers.AddQueryString(refreshUriWithPath.ToString(), refreshUriQuery);

        return refreshUri;
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "GitHub authentication cookie value: {AuthenticationCookie}")]
        public static partial void LogGitHubAuthenticationCookie(ILogger logger, string authenticationCookie);
    }
}
