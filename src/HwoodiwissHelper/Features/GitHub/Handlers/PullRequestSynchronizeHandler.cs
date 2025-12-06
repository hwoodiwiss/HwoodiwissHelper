using System.Diagnostics;
using HwoodiwissHelper.Features.GitHub.Configuration;
using HwoodiwissHelper.Features.GitHub.Events;
using HwoodiwissHelper.Features.GitHub.Events.Models;
using HwoodiwissHelper.Features.GitHub.Services;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Features.GitHub.Handlers;

public sealed partial class PullRequestSynchronizeHandler(IGitHubService gitHubService, IOptions<GitHubConfiguration> githubOptions, ILogger<PullRequestOpenedHandler> logger, ActivitySource activitySource) : Features.GitHub.Handlers.GithubWebhookRequestHandler<PullRequest.Synchronize>(logger, activitySource)
{
    protected override async ValueTask<object?> HandleGithubEventAsync(PullRequest.Synchronize request)
    {
        using var activity = ActivitySource.StartActivity("Handling Pull Request Opened Event");
        Actor pullRequestUser = request.PullRequest.User;

        activity?.SetTag("pullrequest.repository.owner", request.Repository.Owner);
        activity?.SetTag("pullrequest.repository.name", request.Repository.Name);
        activity?.SetTag("pullrequest.number", request.Number);
        activity?.SetTag("pullrequest.user", pullRequestUser.Login);

        if (request.PullRequest.User.Type is ActorType.Bot && githubOptions.Value.AllowedBots.Contains(request.PullRequest.User.Login, StringComparer.OrdinalIgnoreCase))
        {
            Log.BotPullRequestOpened(logger, pullRequestUser.Name);
            await gitHubService.ApprovePullRequestAsync(request.Repository.Owner.Login, request.Repository.Name, request.PullRequest.Number, request.Installation.Id);
        }

        return Results.NoContent();
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Bot pull request opened by {UserName}")]
        public static partial void BotPullRequestOpened(ILogger logger, string userName);
    }
}
