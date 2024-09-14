using System.Net;
using System.Net.Http.Headers;
using HwoodiwissHelper.Features.GitHub.Services;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public sealed partial class GitHubClient(HttpClient httpClient, IGitHubAppAuthProvider authProvider, ILogger<GitHubClient> logger) : IGitHubClient
{
    public async Task<Result<Unit>> CreatePullRequestReview(string repoOwner, string repoName, int pullRequestNumber, int installationId, SubmitReviewRequest reviewRequest)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/repos/{repoOwner}/{repoName}/pulls/{pullRequestNumber}/reviews");
        request.Headers.Accept.Add(new("application/vnd.github+json"));
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

        var installationTokenResult = await GetInstallationToken(
            installationId,
            new Dictionary<InstallationScope, InstallationOperation>
            {
                [InstallationScope.PullRequests] = InstallationOperation.Write
            }, 
            [repoName]);
        
        var installationToken = installationTokenResult.UnwrapSuccess().Value;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", installationToken);
        request.Content = JsonContent.Create(reviewRequest, GitHubClientJsonSerializerContext.Default.SubmitReviewRequest);
        using var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            Log.FailedToApprovePullRequest(logger, pullRequestNumber, repoOwner, repoName, installationId, (int)response.StatusCode);
            return new Problem.Reason("Failed to approve pull request");
        }

        return Unit.Instance;
    }
    
    private async Task<Result<string>> GetInstallationToken(int installationId, Dictionary<InstallationScope, InstallationOperation> permissions, string[]? repositories)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/app/installations/{installationId}/access_tokens");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authProvider.GetGithubJwt());
        request.Headers.Accept.Add(new("application/vnd.github+json"));
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        request.Content = JsonContent.Create(new InstallationTokenRequest
        {
            Repositories = repositories, Permissions = permissions,
        }, GitHubClientJsonSerializerContext.Default.InstallationTokenRequest);
        
        using var response = await httpClient.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.Created)
        {
            Log.InstallationsTokenRequestFailed(logger, installationId, (int)response.StatusCode, await response.Content.ReadAsStringAsync());
            return new Problem.Reason("Failed to create installation token.");
        }

        var result = await response.Content.ReadFromJsonAsync(GitHubClientJsonSerializerContext.Default.InstallationTokenResponse);

        return result?.Token ?? string.Empty;
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Error, "Installation token request for {InstallationId} failed with Status {StatusCode} and Response {ResponseContent}")]
        public static partial void InstallationsTokenRequestFailed(ILogger logger, long installationId, int statusCode, string responseContent);
        
        [LoggerMessage(LogLevel.Error, "Failed to approve pull request #{PullRequest} in {RepoOrg}/{RepoName} for {InstallationId} with Status {StatusCode}")]
        public static partial void FailedToApprovePullRequest(ILogger logger, int pullRequest, string repoOrg, string repoName, int installationId, int statusCode);
    }
}
