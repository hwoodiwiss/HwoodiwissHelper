using GitHub;
using GitHub.Models;
using GitHub.Repos.Item.Item.Pulls.Item.Reviews;
using HwoodiwissHelper.Infrastructure.Github;

namespace HwoodiwissHelper.Services;

public class GithubService(IGithubClientFactory githubClientFactory)
{

    public async Task ApprovePullRequest(string repoOwner, string repoName, int pullRequestNumber, int installationId)
    {
        var permissions = new AppPermissions() {PullRequests = AppPermissions_pull_requests.Write};
        var client = await githubClientFactory.CreateInstallationClient(installationId, permissions);

        if (client is Maybe<GitHubClient>.None) return;

        await client.UnwrapSome().Value.Repos[repoOwner][repoName].Pulls[pullRequestNumber].Reviews.PostAsync(new ReviewsPostRequestBody
        {
            Body = "Automatically approving pull request",
            Event = ReviewsPostRequestBody_event.APPROVE,
        });
    }
}
