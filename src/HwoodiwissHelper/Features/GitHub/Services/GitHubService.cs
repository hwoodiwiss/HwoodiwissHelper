using System.Diagnostics;
using HwoodiwissHelper.Features.GitHub.HttpClients;

namespace HwoodiwissHelper.Features.GitHub.Services;

public sealed partial class GitHubService(IGitHubClient githubClient, ActivitySource activitySource, ILogger<GitHubService> logger) : IGitHubService
{
    public async Task ApprovePullRequestAsync(string repoOwner, string repoName, int pullRequestNumber, int installationId)
    {
        using var activity = activitySource.StartActivity();
        activity?.SetTag("pullrequest.number", pullRequestNumber);
        activity?.SetTag("pullrequest.repo", $"{repoOwner}/{repoName}");
        
        try
        {
            await githubClient.CreatePullRequestReview(repoOwner, repoName, pullRequestNumber, installationId,
                new SubmitReviewRequest
                {
                    Body = "Automatically approving pull request",
                    Event = SubmitReviewEvent.Approve,
                });
        }
        catch (Exception error)
        {
            activity?.SetTag("exception.type", error.GetType().Name);
            Log.FailedToApprovePullRequest(logger, pullRequestNumber, repoOwner, repoName, installationId);
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Error, "Failed to approve pull request #{PullRequest} in {RepoOrg}/{RepoName} for {InstallationId}")]
        public static partial void FailedToApprovePullRequest(ILogger logger, int pullRequest, string repoOrg, string repoName, int installationId);
    }
}
