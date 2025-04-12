using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using HwoodiwissHelper.Core;
using HwoodiwissHelper.Core.Features.Models;
using HwoodiwissHelper.Extensions;
using HwoodiwissHelper.Features.GitHub.Configuration;
using HwoodiwissHelper.Features.GitHub.Events;
using HwoodiwissHelper.Features.GitHub.Extension;
using HwoodiwissHelper.Features.GitHub.Filters;
using HwoodiwissHelper.Features.GitHub.HttpClients;
using HwoodiwissHelper.Handlers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Features.GitHub.Endpoints;

public static partial class GitHubEndpoints
{
    public static IEndpointRouteBuilder MapGitHubEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/github");

        group.MapPost("/webhook", async (
                [FromKeyedServices(nameof(GitHubEndpoints))] ILogger logger,
                [FromHeader(Name = "X-Github-Event")] string githubEvent,
                [FromServices] IOptions<JsonOptions> jsonOptions,
                HttpRequest request,
                IServiceProvider serviceProvider,
                IOptionsSnapshot<GitHubConfiguration> githubConfiguration) =>
            {
                using var _ = logger.BeginScope(new Dictionary<string, object>
                {
                    ["GitHubEvent"] = githubEvent,
                    ["GitHubEndpoint"] = "webhook",
                });

                if (githubConfiguration.Value.EnableRequestLogging)
                {
                    var githubEventBody = await GetRequestBodyText(request.Body);
                    Log.ReceivedGithubEvent(logger, githubEventBody);
                    request.Body.Seek(0, SeekOrigin.Begin);
                }

                var githubEventBase = await GetGithubEvent(logger, githubEvent, request.Body);

                var requestHandler = serviceProvider.GetKeyedService<IRequestHandler<GitHubWebhookEvent>>(githubEventBase?.GetType());

                if (githubEventBase is null || requestHandler is null)
                    return Results.NoContent();

                return await requestHandler.HandleAsync(githubEventBase);
            })
            .WithBufferedRequest()
            .AddEndpointFilterFactory(GitHubSecretValidatorFilter.Factory)
            .Produces(201);

        group.MapGet("/login", HandleLoginRequest)
            .Produces(302)
            .Produces(400);
        
        group.MapGet("/login/callback", HandleLoginCallback)
            .Produces(302)
            .Produces(400);

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
            var githubEventBody = await GetRequestBodyText(body);
            Log.DeserializationFailed(logger, githubEventBody, ex);
            return null;
        }
        catch (NotSupportedException ex)
        {
            var githubEventBody = await GetRequestBodyText(body);
            Log.DeserializingGithubEventNotSupported(logger, githubEventBody, ex);
            return null;
        }
    }

    private static async Task<string> GetRequestBodyText(Stream requestBody)
    {
        requestBody.Seek(0, SeekOrigin.Begin);
        return await new StreamReader(requestBody).ReadToEndAsync();
    }

    private static RedirectHttpResult HandleLoginRequest(
        [FromQuery] string redirectUri,
        [FromKeyedServices(nameof(GitHubEndpoints))] ILogger logger,
        IOptionsSnapshot<GitHubConfiguration> githubConfiguration)
    {
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["GitHubEndpoint"] = "login",
        });

        var clientId = githubConfiguration.Value.ClientId;

        var authorizeQs = new Dictionary<string, string?>
        {
            ["client_id"] = clientId,
            ["redirect_uri"] = redirectUri,
        };
        
        Log.LogLoginRedirectUri(logger, redirectUri);
        
        var authorizeUrl = QueryHelpers.AddQueryString("https://github.com/login/oauth/authorize", authorizeQs);

        return TypedResults.Redirect(authorizeUrl);
    }
    
    private static bool TryValidateRedirectUri(string redirectUri, string[] allowedRedirectHosts, [NotNullWhen(true)] out Uri? validateRedirectUri)
    {
        validateRedirectUri = null;
        if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (allowedRedirectHosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase))
        {
            validateRedirectUri = uri;
            return true;
        }

        return false;
    }
    
    private static async Task<IResult> HandleLoginCallback(
        [FromQuery] string redirectUri,
        [FromQuery] string code,
        [FromKeyedServices(nameof(GitHubEndpoints))] ILogger logger,
        HttpResponse response,
        IGitHubClient gitHubClient,
        IOptionsSnapshot<GitHubConfiguration> githubConfiguration,
        IHostEnvironment environment)
    {
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["GitHubEndpoint"] = "login-callback",
        });
        
        Log.LogCallbackRedirectUri(logger, redirectUri);

        var authResult = await gitHubClient.AuthorizeUserAsync(code, redirectUri);
        
        if (!TryValidateRedirectUri(redirectUri, githubConfiguration.Value.AllowedRedirectHosts, out var validateRedirectUri))
        {
            Log.DisallowedRedirectUri(logger, redirectUri);
            return TypedResults.BadRequest();
        }
        
        return authResult switch
        {
            Result<AuthorizeUserResponse, Problem>.Success { Value: {} resultValue } => RedirectOnAuthSuccess(response, resultValue, validateRedirectUri, environment),
            _ => TypedResults.BadRequest(),
        };
    }

    private static RedirectHttpResult RedirectOnAuthSuccess(HttpResponse response, AuthorizeUserResponse authResponse, Uri redirectUri, IHostEnvironment environment)
    {
        var authCookieOptions = new CookieOptions
        {
            HttpOnly = false,
            Secure = environment.IsProduction(),
            Expires = DateTimeOffset.UtcNow.AddDays(180),
        };

        if (!redirectUri.IsLoopback)
        {
            var cookieDomain = new StringBuilder(redirectUri.Host);
            if (redirectUri.Port != 80 && redirectUri.Port != 443)
            {
                cookieDomain.Append(':').Append(redirectUri.Port);
            }
            
            authCookieOptions.Domain = cookieDomain.ToString();
        }

        var userAuthDetails = authResponse.ToUserAuthDetails();
        
        var authCookieB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(userAuthDetails, GitHubJsonSerializerContext.Default.UserAuthenticationDetails)));
        
        response.Cookies.Append("github_auth", authCookieB64, authCookieOptions);

        return TypedResults.Redirect(redirectUri.ToString());
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Received login request with RedirectUri {RedirectUri}")]
        public static partial void LogLoginRedirectUri(ILogger logger, string redirectUri);
        
        [LoggerMessage(LogLevel.Information, "Received login callback with RedirectUri {RedirectUri}")]
        public static partial void LogCallbackRedirectUri(ILogger logger, string redirectUri);
        
        [LoggerMessage(LogLevel.Warning, "Failed to deserialize github event {GithubEventBody}")]
        public static partial void DeserializationFailed(ILogger logger, string githubEventBody, Exception exception);

        [LoggerMessage(LogLevel.Error, "Failed to deserialize github event data {GithubEventBody}")]
        public static partial void DeserializingGithubEventNotSupported(ILogger logger, string githubEventBody, Exception exception);

        [LoggerMessage(LogLevel.Information, "Received Github event: {GithubEventBody}")]
        public static partial void ReceivedGithubEvent(ILogger logger, string githubEventBody);

        [LoggerMessage(LogLevel.Warning, "Attempted to redirect to invalid URI {RedirectUri}")]
        public static partial void DisallowedRedirectUri(ILogger logger, string redirectUri);
    }
}
