using GitHub;
using GitHub.Models;

namespace HwoodiwissHelper.Infrastructure.Github;

public interface IGithubClientFactory
{
    GitHubClient CreateAppClient();
    Task<Option<GitHubClient>> CreateInstallationClient(int installationId, AppPermissions? permissions = null);
}
