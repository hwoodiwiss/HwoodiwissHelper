using HwoodiwissHelper.Events.Github;

namespace HwoodiwissHelper.Handlers.Github;

public sealed partial class WorkflowCompleteHandler(ILogger<WorkflowCompleteHandler> logger) : GithubWebhookRequestHandler<WorkflowRun.Completed>(logger)
{
    protected override ValueTask<object?> HandleGithubEventAsync(WorkflowRun.Completed request)
    {
        return new ValueTask<object?>(Results.NoContent());
    }
}
