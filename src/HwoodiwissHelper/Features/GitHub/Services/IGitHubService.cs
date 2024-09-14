namespace HwoodiwissHelper.Features.GitHub.Services;

public interface IGitHubService
{
    Task ApprovePullRequestAsync(string repoOwner, string repoName, int pullRequestNumber, int installationId);
}
