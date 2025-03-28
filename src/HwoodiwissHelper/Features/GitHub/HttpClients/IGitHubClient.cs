using HwoodiwissHelper.Core;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public interface IGitHubClient
{
    Task<Result<Unit, Problem>> CreatePullRequestReview(string repoOwner, string repoName, int pullRequestNumber, int installationId, SubmitReviewRequest reviewRequest);

    Task<Result<AuthorizeUserResponse, Problem>> AuthorizeUserAsync(string code, string redirectUri);
}
