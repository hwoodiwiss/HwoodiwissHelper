using System.Text.Json;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Extensions;
using HwoodiwissHelper.Handlers;
using HwoodiwissHelper.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Endpoints;

public static partial class GithubWebhookEndpoints
{
    public static IEndpointRouteBuilder MapGithubEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/github");

        group.MapPost("/webhook", async (
                [FromKeyedServices(nameof(GithubWebhookEndpoints))] ILogger logger,
                [FromHeader(Name = "X-Github-Event")] string githubEvent, 
                [FromServices] IOptions<JsonOptions> jsonOptions,
                HttpRequest request,
                IServiceProvider serviceProvider) =>
            {
                using var _ = logger.BeginScope(new Dictionary<string, object>{
                    ["GithubEvent"] = githubEvent,
                });

                var githubEventBase = await GetGithubEvent(logger, githubEvent, request.Body);
                
                var requestHandler = serviceProvider.GetKeyedService<IRequestHandler<GithubWebhookEvent>>(githubEventBase?.GetType());

                if (githubEventBase is null || requestHandler is null)
                    return Results.NoContent();
                
                return await requestHandler.HandleAsync(githubEventBase);
            })
            .WithBufferedRequest()
            .AddEndpointFilterFactory(GithubSecretValidatorFilter.Factory);
        
        return builder;
    }
    
    private static async Task<GithubWebhookEvent?> GetGithubEvent(ILogger logger, string githubEvent, Stream body)
    {
        try
        {
            return githubEvent switch
            {
                "workflow_run" => (GithubWebhookEvent?)await JsonSerializer.DeserializeAsync(body, ApplicationJsonContext.Default.WorkflowRun),
                _ => null,
            };
        }
        catch (JsonException ex)
        {
            Log.DeserializationFailed(logger, githubEvent, ex);
            return null;
        }
    }
    
    private static partial class Log
    {
        [LoggerMessage(LogLevel.Error, "Failed to deserialize github event {GithubEvent}")]
        public static partial void DeserializationFailed(ILogger logger, string githubEvent, Exception exception);
    }
}
