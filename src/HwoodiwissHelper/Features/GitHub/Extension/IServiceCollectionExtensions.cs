using System.Diagnostics.CodeAnalysis;
using HwoodiwissHelper.Features.GitHub.Events;
using HwoodiwissHelper.Features.GitHub.Handlers;
using HwoodiwissHelper.Features.GitHub.Services;
using HwoodiwissHelper.Handlers;

namespace HwoodiwissHelper.Features.GitHub.Extension;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddGithubHandlerServices(this IServiceCollection services)
    {
        services.AddSingleton<IGitHubSignatureValidator, GitHubSignatureValidator>();
        services.AddSingleton<IGitHubAppAuthProvider, GitHubAppAuthProvider>();
        services.AddScoped<IGitHubClientFactory, GitHubClientFactory>();
        services.AddScoped<IGitHubService, GitHubService>();
        services.AddGithubWebhookHandlers();

        return services;
    }
    
    private static IServiceCollection AddGithubWebhookHandlers(this IServiceCollection services)
    {
        services.AddGithubEventHandler<WorkflowCompleteHandler, WorkflowRun.Completed>();
        services.AddGithubEventHandler<PullRequestOpenedHandler, PullRequest.Opened>();
        services.AddGithubEventHandler<PullRequestSynchronizeHandler, PullRequest.Synchronize>();

        return services;
    }
    
    private static IServiceCollection AddGithubEventHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler, TEvent>(this IServiceCollection services)
        where TEvent : GitHubWebhookEvent
        where THandler : Features.GitHub.Handlers.GithubWebhookRequestHandler<TEvent>
    {
        services.AddKeyedScoped<IRequestHandler<GitHubWebhookEvent>, THandler>(typeof(TEvent));
        return services;
    }
}
