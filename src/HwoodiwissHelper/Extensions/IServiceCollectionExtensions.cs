using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Handlers;
using HwoodiwissHelper.Handlers.Github;
using WorkflowCompleteHandler = HwoodiwissHelper.Handlers.Github.WorkflowCompleteHandler;

namespace HwoodiwissHelper.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddGithubWebhookHandlers(this IServiceCollection services)
    {
        services.AddGithubEventHandler<WorkflowCompleteHandler, WorkflowRun.Complete>();
        
        return services;
    }

    private static IServiceCollection AddGithubEventHandler<THandler, TEvent>(this IServiceCollection services)
        where TEvent : GithubWebhookEvent
        where THandler : GithubWebhookRequestHandler<TEvent>
    {
        services.AddKeyedScoped<IRequestHandler<GithubWebhookEvent>, THandler>(typeof(TEvent));
        return services;
    }
}
