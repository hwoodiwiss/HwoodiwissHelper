using System.Diagnostics;
using HwoodiwissHelper.Configuration;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Models.Github;
using HwoodiwissHelper.Services;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Handlers.Github;

public sealed partial class PullRequestOpenedHandler(IGithubService githubService, IOptions<GithubConfiguration> githubOptions, ILogger<PullRequestOpenedHandler> logger, ActivitySource activitySource) : GithubWebhookRequestHandler<PullRequest.Opened>(logger, activitySource)
{
    protected override async ValueTask<object?> HandleGithubEventAsync(PullRequest.Opened request)
    {
        using var activity = ActivitySource.StartActivity("Handling Pull Request Opened Event");
        Actor pullRequestUser = request.PullRequest.User;

        activity?.SetTag("pullrequest.repository.owner", request.Repository.Owner);
        activity?.SetTag("pullrequest.repository.name", request.Repository.Name);
        activity?.SetTag("pullrequest.number", request.Number);
        activity?.SetTag("pullrequest.user", pullRequestUser.Name);

        if (request.PullRequest.User.Type is ActorType.Bot && githubOptions.Value.AllowedBots.Contains(request.PullRequest.User.Login, StringComparer.OrdinalIgnoreCase))
        {
            Log.BotPullRequestOpened(logger, pullRequestUser.Name);
            return Results.NoContent();
        }
        {
            Log.BotPullRequestOpened(logger, pullRequestUser.Name);
            await githubService.ApprovePullRequestAsync(request.Repository.Owner.Login, request.Repository.Name, request.PullRequest.Number, request.Installation.Id);
        }

        return Results.NoContent();
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Bot pull request opened by {UserName}")]
        public static partial void BotPullRequestOpened(ILogger logger, string userName);
    }
}
