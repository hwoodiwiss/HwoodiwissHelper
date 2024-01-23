using System.Diagnostics;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Models.Github;
using HwoodiwissHelper.Services;

namespace HwoodiwissHelper.Handlers.Github;

public sealed partial class PullRequestOpenedHandler(IGithubService githubService, ILogger<PullRequestOpenedHandler> logger, ActivitySource activitySource) : GithubWebhookRequestHandler<PullRequest.Opened>(logger, activitySource)
{
    protected override async ValueTask<object?> HandleGithubEventAsync(PullRequest.Opened request)
    {
        using var activity = ActivitySource.StartActivity("Handling Pull Request Opened Event");
        Actor pullRequestUser = request.PullRequest.User;

        activity?.SetTag("pullrequest.number", request.Number);
        activity?.SetTag("pullrequest.user", pullRequestUser.Name);

        if (request.PullRequest.User.Type is ActorType.Bot)
        {
            Log.BotPullRequestOpened(logger, pullRequestUser.Name);
            await githubService.ApprovePullRequestAsync(request.Repository.Owner.Login, request.Repository.Name, request.PullRequest.Number, request.Installation.Id);
        }

        return new ValueTask<object?>(Results.NoContent());
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Bot pull request opened by {UserName}")]
        public static partial void BotPullRequestOpened(ILogger logger, string userName);
    }
}
