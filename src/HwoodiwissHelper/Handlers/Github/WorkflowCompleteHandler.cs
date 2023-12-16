using HwoodiwissHelper.Events.Github;

namespace HwoodiwissHelper.Handlers.Github;

public sealed partial class WorkflowCompleteHandler(ILogger<WorkflowCompleteHandler> logger) : GithubWebhookRequestHandler<WorkflowRun.Completed>(logger)
{
    protected override async ValueTask<object?> HandleGithubEventAsync(WorkflowRun.Completed request)
    {
        await Task.Delay(10);
        return Results.NoContent();
    }
}
