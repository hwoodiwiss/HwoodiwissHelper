using System.Text.Json;
using Hwoodiwiss.Extensions.Hosting.Extensions;
using HwoodiwissHelper.Features.GitHub.Events;
using HwoodiwissHelper.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Features.Benchmarks.Endpoints;

public static partial class BenchmarkEndpoints
{
    public static T MapBenchmarkEndpoints<T>(this T builder) where T : IEndpointRouteBuilder
    {
        var group = builder.MapGroup("/benchmark");

        group.MapPost("/webhook", async (
                [FromKeyedServices(nameof(BenchmarkEndpoints))] ILogger logger,
                [FromHeader(Name = "X-Github-Event")] string githubEvent,
                [FromServices] IOptions<JsonOptions> jsonOptions,
                HttpRequest request,
                IServiceProvider serviceProvider) =>
            {
                using var _ = logger.BeginScope(new Dictionary<string, object>
                {
                    ["GithubEvent"] = githubEvent,
                });

                var githubEventBase = await GetGithubEvent(logger, githubEvent, request.Body);

                var requestHandler = serviceProvider.GetKeyedService<IRequestHandler<GitHubWebhookEvent>>(githubEventBase?.GetType());

                if (githubEventBase is null || requestHandler is null)
                    return Results.NoContent();

                return await requestHandler.HandleAsync(githubEventBase);
            })
            .WithBufferedRequest()
            .Produces(201)
            .WithTags("Benchmarks")
            .WithDescription("Benchmark endpoint that mirrors the GitHub webhook pipeline without HMAC validation. Requires a configured Bearer API key.");

        return builder;
    }

    private static async Task<GitHubWebhookEvent?> GetGithubEvent(ILogger logger, string githubEvent, Stream body)
    {
        try
        {
            return githubEvent switch
            {
                "workflow_run" => await JsonSerializer.DeserializeAsync(body, ApplicationJsonContext.Default.WorkflowRun),
                "pull_request" => await JsonSerializer.DeserializeAsync(body, ApplicationJsonContext.Default.PullRequest),
                _ => null,
            };
        }
        catch (JsonException ex)
        {
            Log.DeserializationFailed(logger, ex);
            return null;
        }
        catch (NotSupportedException ex)
        {
            Log.DeserializingGithubEventNotSupported(logger, ex);
            return null;
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Warning, "Failed to deserialize benchmark webhook event body")]
        public static partial void DeserializationFailed(ILogger logger, Exception exception);

        [LoggerMessage(LogLevel.Error, "Deserializing benchmark webhook event is not supported")]
        public static partial void DeserializingGithubEventNotSupported(ILogger logger, Exception exception);
    }
}
