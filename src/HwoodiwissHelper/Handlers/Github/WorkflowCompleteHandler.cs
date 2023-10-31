using HwoodiwissHelper.Events.Github;

namespace HwoodiwissHelper.Handlers.Github;

public sealed partial class WorkflowCompleteHandler(ILogger<WorkflowCompleteHandler> logger) : GithubWebhookRequestHandler<WorkflowRun.Complete>(logger)
{
    protected override async ValueTask<object?> HandleGithubEventAsync(WorkflowRun.Complete request)
    {
        Log.Enter(logger);
        await Task.Delay(10);
        return Results.NoContent();
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Received webhook event with some data")]
        public static partial void Enter(ILogger logger);
    }
}
