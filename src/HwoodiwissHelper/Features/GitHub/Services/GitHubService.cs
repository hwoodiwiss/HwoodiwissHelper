using System.Diagnostics;
using GitHub;
using GitHub.Models;
using GitHub.Repos.Item.Item.Pulls.Item.Reviews;
using Microsoft.Kiota.Abstractions;

namespace HwoodiwissHelper.Features.GitHub.Services;

public sealed partial class GitHubService(IGitHubClientFactory gitHubClientFactory, ActivitySource activitySource, ILogger<GitHubService> logger) : IGitHubService
{

    public async Task ApprovePullRequestAsync(string repoOwner, string repoName, int pullRequestNumber, int installationId)
    {
        using var activity = activitySource.StartActivity();
        activity?.SetTag("pullrequest.number", pullRequestNumber);
        activity?.SetTag("pullrequest.repo", $"{repoOwner}/{repoName}");

        var permissions = new AppPermissions() { PullRequests = AppPermissions_pull_requests.Write };
        var client = await gitHubClientFactory.CreateInstallationClient(installationId, permissions);

        if (client is Option<GitHubClient>.None) return;

        try
        {
            await client.UnwrapSome().Value.Repos[repoOwner][repoName].Pulls[pullRequestNumber].Reviews.PostAsync(
                new ReviewsPostRequestBody
                {
                    Body = "Automatically approving pull request",
                    Event = ReviewsPostRequestBody_event.APPROVE,
                });
        }
        catch (Exception error)
        {
            activity?.SetTag("exception.type", error.GetType().Name);
            if (error is ApiException apiException)
            {
                activity?.SetTag("exception.status-code", apiException.ResponseStatusCode);
            }
            Log.FailedToApprovePullRequest(logger, pullRequestNumber, repoOwner, repoName, installationId);
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Error, "Failed to approve pull request #{PullRequest} in {RepoOrg}/{RepoName} for {InstallationId}")]
        public static partial void FailedToApprovePullRequest(ILogger logger, int pullRequest, string repoOrg, string repoName, int installationId);
    }
}
