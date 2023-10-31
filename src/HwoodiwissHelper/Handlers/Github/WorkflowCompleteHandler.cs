using HwoodiwissHelper.Events.Github;

namespace HwoodiwissHelper.Handlers.Github;

public sealed partial class WorkflowCompleteHandler(ILogger<WorkflowCompleteHandler> logger) : GithubWebhookRequestHandler<WorkflowRun.Complete>(logger)
{
    protected override async ValueTask<object?> HandleGithubEventAsync(WorkflowRun.Complete request)
    {
        await Task.Delay(10);
        return Results.NoContent();
    }
}
