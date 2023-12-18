using HwoodiwissHelper.Events.Github;

namespace HwoodiwissHelper.Handlers.Github;

public sealed partial class PullRequestOpenedHandler(ILogger<PullRequestOpenedHandler> logger) : GithubWebhookRequestHandler<PullRequest.Opened>(logger)
{
    protected override ValueTask<object?> HandleGithubEventAsync(PullRequest.Opened request)
    {
        return new ValueTask<object?>(Results.NoContent());
    }
}
