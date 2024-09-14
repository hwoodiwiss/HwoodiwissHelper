using System.Diagnostics;
using HwoodiwissHelper.Features.GitHub.Events;

namespace HwoodiwissHelper.Features.GitHub.Handlers;

public sealed partial class WorkflowCompleteHandler(
    ILogger<WorkflowCompleteHandler> logger,
    ActivitySource activitySource)
    : GithubWebhookRequestHandler<WorkflowRun.Completed>(logger, activitySource)
{
    protected override ValueTask<object?> HandleGithubEventAsync(WorkflowRun.Completed request)
    {
        return new ValueTask<object?>(Results.NoContent());
    }
}
