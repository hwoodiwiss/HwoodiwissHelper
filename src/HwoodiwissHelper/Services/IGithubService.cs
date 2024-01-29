namespace HwoodiwissHelper.Services;

public interface IGithubService
{
    Task ApprovePullRequestAsync(string repoOwner, string repoName, int pullRequestNumber, int installationId);
}
