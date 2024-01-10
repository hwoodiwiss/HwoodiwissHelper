using System.Diagnostics;
using HwoodiwissHelper.Events.Github;

namespace HwoodiwissHelper.Handlers.Github;

public abstract partial class GithubWebhookRequestHandler<T>(ILogger logger, ActivitySource activitySource) : IRequestHandler<GithubWebhookEvent>
    where T : GithubWebhookEvent
{
    protected Type EventType { get; } = typeof(T);
    protected ActivitySource ActivitySource { get; } = activitySource;
    
    public async ValueTask<object?> HandleAsync(GithubWebhookEvent request)
    {
        using var activity = ActivitySource.StartActivity("Handling Github Webhook Event");
        activity?.SetTag("EventType", EventType.Name);
        activity?.SetTag("Handler", GetType().Name);
        activity?.SetTag("SenderName", request.Sender.Name);
        activity?.SetTag("SenderType", request.Sender.Type);
            
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
