using HwoodiwissHelper.Events.Github;

namespace HwoodiwissHelper.Handlers.Github;

public abstract partial class GithubWebhookRequestHandler<T>(ILogger logger) : IRequestHandler<GithubWebhookEvent>
    where T : GithubWebhookEvent
{
    protected Type EventType { get; } = typeof(T);
    
    public async ValueTask<object?> HandleAsync(GithubWebhookEvent request)
    {
        Log.HandlingEvent(logger, EventType);
        
        return request is T matchingRequestType
            ? await HandleGithubEventAsync(matchingRequestType)
            : throw new InvalidOperationException("The provided event type did not match the required type for the current handler.");
    }

    protected abstract ValueTask<object?> HandleGithubEventAsync(T request);

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Handling webhook event for {EventType}")]
        public static partial void HandlingEvent(ILogger logger, Type eventType);
    }
}
