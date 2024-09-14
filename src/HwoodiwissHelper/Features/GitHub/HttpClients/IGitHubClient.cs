namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public interface IGitHubClient
{
    Task<Result<Unit>> CreatePullRequestReview(string repoOwner, string repoName, int pullRequestNumber, int installationId, SubmitReviewRequest reviewRequest);
}
