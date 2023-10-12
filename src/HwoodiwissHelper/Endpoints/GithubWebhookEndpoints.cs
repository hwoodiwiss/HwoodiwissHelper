using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Extensions;
using HwoodiwissHelper.Handlers;
using HwoodiwissHelper.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Endpoints;

public static class GithubWebhookEndpoints
{
    public static IEndpointRouteBuilder MapGithubEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/github");

        group.MapPost("/webhook", async ([FromHeader(Name = "X-Github-Event")] string githubEvent, 
                [FromServices] IOptions<JsonOptions> jsonOptions,
                HttpRequest request) =>
            {
                var jso = jsonOptions.Value;
                var githubEventBase = githubEvent switch
                {
                    "workflow_run" => (GithubWebhookEvent?)await JsonSerializer.DeserializeAsync<WorkflowRun>(request.Body, jso.JsonSerializerOptions),
                    _ => null,
                } ?? throw new NotSupportedException();
                
                return githubEventBase.GetType().FullName;
            })
            .WithBufferedRequest()
            .AddEndpointFilterFactory(GithubSecretValidatorFilter.Factory);
        
        return builder;
    }
}
