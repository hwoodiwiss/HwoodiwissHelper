using GitHub;
using GitHub.Models;

namespace HwoodiwissHelper.Features.GitHub.Services;

public interface IGitHubClientFactory
{
    GitHubClient CreateAppClient();
    Task<Option<GitHubClient>> CreateInstallationClient(int installationId, AppPermissions? permissions = null);
}
