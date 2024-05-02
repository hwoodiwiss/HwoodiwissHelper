using GitHub;
using GitHub.App.Installations.Item.Access_tokens;
using GitHub.Models;
using GitHub.Octokit.Authentication;
using GitHub.Octokit.Client;

namespace HwoodiwissHelper.Infrastructure.Github;

public partial class GithubClientFactory(ILogger<GithubClientFactory> logger, HttpClient client, IGithubAppAuthProvider authProvider) : IGithubClientFactory
{
    public GitHubClient CreateAppClient()
    {
        var jwt = authProvider.GetGithubJwt();
        var adapter = RequestAdapter.Create(new TokenAuthenticationProvider(ApplicationMetadata.Name, jwt), client);
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
                    new TokenAuthenticationProvider(ApplicationMetadata.Name, accessToken?.Token ?? string.Empty),
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
