﻿using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using HwoodiwissHelper.Features.GitHub.Configuration;
using HwoodiwissHelper.Features.GitHub.Events;
using HwoodiwissHelper.Features.GitHub.Handlers;
using HwoodiwissHelper.Features.GitHub.HttpClients;
using HwoodiwissHelper.Features.GitHub.Services;
using HwoodiwissHelper.Handlers;

namespace HwoodiwissHelper.Features.GitHub.Extension;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureGitHubServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GitHubConfiguration>(configuration.GetSection(GitHubConfiguration.SectionName));
        services.PostConfigure<GitHubConfiguration>(config =>
        {
            foreach (var appConfig in config.AppConfigurations.Values)
            {
                var approxDecodedLength = appConfig.PrivateKey.Length / 4 * 3; // Base64 is roughly 4 bytes per 3 chars
                Span<byte> buffer = ArrayPool<byte>.Shared.Rent(approxDecodedLength);
                if (Convert.TryFromBase64String(appConfig.PrivateKey, buffer, out var bytesWritten))
                {
                    appConfig.PrivateKey = Encoding.UTF8.GetString(buffer[..bytesWritten]);
                }
            }
        });
        
        services.AddSingleton<IGitHubSignatureValidator, GitHubSignatureValidator>();
        services.AddSingleton<IGitHubAppAuthProvider, GitHubAppAuthProvider>();
        services.AddScoped<IGitHubService, GitHubService>();
        services.AddGitHubWebhookHandlers();

        services.AddHttpClient<IGitHubClient, GitHubClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HwoodiwissHelper", $"{ApplicationMetadata.Version}+{ApplicationMetadata.GitCommit}"));
        });

        return services;
    }
    
    private static IServiceCollection AddGitHubWebhookHandlers(this IServiceCollection services)
    {
        services.AddGitHubEventHandler<WorkflowCompleteHandler, WorkflowRun.Completed>();
        services.AddGitHubEventHandler<PullRequestOpenedHandler, PullRequest.Opened>();
        services.AddGitHubEventHandler<PullRequestSynchronizeHandler, PullRequest.Synchronize>();

        return services;
    }
    
    private static IServiceCollection AddGitHubEventHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler, TEvent>(this IServiceCollection services)
        where TEvent : GitHubWebhookEvent
        where THandler : Features.GitHub.Handlers.GithubWebhookRequestHandler<TEvent>
    {
        services.AddKeyedScoped<IRequestHandler<GitHubWebhookEvent>, THandler>(typeof(TEvent));
        return services;
    }
}
