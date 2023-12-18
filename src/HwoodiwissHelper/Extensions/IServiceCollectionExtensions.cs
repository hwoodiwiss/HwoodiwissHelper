using System.Diagnostics.CodeAnalysis;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Handlers;
using HwoodiwissHelper.Handlers.Github;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WorkflowCompleteHandler = HwoodiwissHelper.Handlers.Github.WorkflowCompleteHandler;

namespace HwoodiwissHelper.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(TelemetryResourceBuilder)
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddOtlpExporter();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter();
            });

        static void TelemetryResourceBuilder(ResourceBuilder resourceBuilder)
        {
            resourceBuilder
                .AddService(ApplicationMetadata.Name)
                .AddAttributes([
                    new ("service.commit", ApplicationMetadata.GitCommit),
                    new ("service.branch", ApplicationMetadata.GitBranch),
                    new ("service.version", ApplicationMetadata.Version),
                    new ("service.host", Environment.MachineName),
                ]);
        }
        
        return services;
    }
    
    public static IServiceCollection AddGithubWebhookHandlers(this IServiceCollection services)
    {
        services.AddGithubEventHandler<WorkflowCompleteHandler, WorkflowRun.Completed>();
        services.AddGithubEventHandler<PullRequestOpenedHandler, PullRequest.Opened>();
        
        return services;
    }

    private static IServiceCollection AddGithubEventHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler, TEvent>(this IServiceCollection services)
        where TEvent : GithubWebhookEvent
        where THandler : GithubWebhookRequestHandler<TEvent>
    {
        services.AddKeyedScoped<IRequestHandler<GithubWebhookEvent>, THandler>(typeof(TEvent));
        return services;
    }
}
