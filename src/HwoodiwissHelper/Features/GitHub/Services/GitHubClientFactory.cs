using GitHub;
using GitHub.App.Installations.Item.Access_tokens;
using GitHub.Models;
using GitHub.Octokit.Client;
using GitHub.Octokit.Client.Authentication;

namespace HwoodiwissHelper.Features.GitHub.Services;

public partial class GitHubClientFactory(ILogger<GitHubClientFactory> logger, HttpClient client, IGitHubAppAuthProvider authProvider) : IGitHubClientFactory
{
    public GitHubClient CreateAppClient()
    {
        var jwt = authProvider.GetGithubJwt();
        var adapter = RequestAdapter.Create(new TokenAuthProvider(new TokenProvider(jwt)), client);
        return new GitHubClient(adapter);
    }

    public async Task<Option<GitHubClient>> CreateInstallationClient(int installationId, AppPermissions? permissions)
    {
        try
        {
            var appClient = CreateAppClient();
            var accessToken = await appClient.App.Installations[installationId].Access_tokens
                .PostAsync(new Access_tokensPostRequestBody
                {
                    Permissions = permissions
                });

            if (string.IsNullOrWhiteSpace(accessToken?.Token))
            {
                return new Option<GitHubClient>.None();
            }

            var adapter =
                RequestAdapter.Create(
                    new TokenAuthProvider(new TokenProvider(accessToken?.Token ?? string.Empty)),
                    client);
            return new Option<GitHubClient>.Some(new GitHubClient(adapter));
        }
        catch (Exception ex)
        {
            Log.AccessTokenRequestFailed(logger, installationId, ex.GetType().Name, ex);
            return new Option<GitHubClient>.None();
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Warning, "Failed to create GitHub client for installation {InstallationId} with {ErrorName}")]
        public static partial void AccessTokenRequestFailed(ILogger logger, int installationId, string errorName, Exception ex);
    }
}
