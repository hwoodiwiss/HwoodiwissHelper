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
        activity?.SetTag("event.repository", EventType.Name);
        activity?.SetTag("event.type", EventType.Name);
        activity?.SetTag("event.handler", GetType().Name);
        activity?.SetTag("event.sender.login", request.Sender.Login);
        activity?.SetTag("event.sender.type", request.Sender.Type);
        activity?.SetTag("event.installation.id", request.Installation.Id);

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
