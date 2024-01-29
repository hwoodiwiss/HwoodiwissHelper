using GitHub;
using GitHub.Models;

namespace HwoodiwissHelper.Infrastructure.Github;

public interface IGithubClientFactory
{
    GitHubClient CreateAppClient();
    Task<Maybe<GitHubClient>> CreateInstallationClient(int installationId, AppPermissions? permissions = null);
}
