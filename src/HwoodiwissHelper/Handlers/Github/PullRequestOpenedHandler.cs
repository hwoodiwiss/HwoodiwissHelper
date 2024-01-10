using System.Diagnostics;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper.Handlers.Github;

public sealed partial class PullRequestOpenedHandler(ILogger<PullRequestOpenedHandler> logger, ActivitySource activitySource) : GithubWebhookRequestHandler<PullRequest.Opened>(logger, activitySource)
{
    protected override ValueTask<object?> HandleGithubEventAsync(PullRequest.Opened request)
    {
        using var activity = ActivitySource.StartActivity("Handling Pull Request Opened Event");
        Actor pullRequestUser = request.PullRequest.User;
        
        activity?.SetTag("PullRequestNumber", request.Number);
        activity?.SetTag("PullRequestUser", pullRequestUser.Name);
        
        if (request.PullRequest.User.Type is ActorType.Bot)
        {
            Log.BotPullRequestOpened(logger, pullRequestUser.Name);
        }
        
        return new ValueTask<object?>(Results.NoContent());
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Bot pull request opened by {UserName}")]
        public static partial void BotPullRequestOpened(ILogger logger, string userName);
    }
}
