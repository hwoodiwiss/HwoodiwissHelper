using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using HwoodiwissHelper.Configuration;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Handlers;
using HwoodiwissHelper.Handlers.Github;
using HwoodiwissHelper.Infrastructure.Github;
using HwoodiwissHelper.Middleware;
using HwoodiwissHelper.Services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WorkflowCompleteHandler = HwoodiwissHelper.Handlers.Github.WorkflowCompleteHandler;

namespace HwoodiwissHelper.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<GithubConfiguration>(configuration.GetSection(GithubConfiguration.SectionName));
        services.PostConfigure<GithubConfiguration>(config =>
        {
            var approxDecodedLength = config.AppPrivateKey.Length / 4 * 3; // Base64 is roughly 4 bytes per 3 chars
            Span<byte> buffer = approxDecodedLength < 2000 ? stackalloc byte[approxDecodedLength] : new byte[approxDecodedLength];
            if (Convert.TryFromBase64String(config.AppPrivateKey, buffer, out var bytesWritten))
            {
                config.AppPrivateKey = Encoding.UTF8.GetString(buffer[..bytesWritten]);
            }
        });
        services.Configure<ApplicationConfiguration>(configuration);

        return services;
    }

    public static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(TelemetryResourceBuilder)
            .WithMetrics(metrics =>
            {
                metrics.AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("Microsoft.AspNetCore.Diagnostics")
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
                    new ("service.aot", !(RuntimeFeature.IsDynamicCodeSupported & RuntimeFeature.IsDynamicCodeCompiled)),
                ])
                .AddContainerDetector()
                .AddHostDetector();
        }

        return services;
    }

    public static IServiceCollection ConfigureHttpClients(this IServiceCollection services)
    {
        services.ConfigureHttpClientDefaults(builder =>
        {
            builder.ConfigureHttpClient(client =>
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(ApplicationMetadata.Name,
                    ApplicationMetadata.Version));

            });

            // Note - Disabled for now due to an issue with AOT compatibility
            // builder.AddStandardResilienceHandler();
        });

        services.AddHttpClient();

        return services;
    }

    public static IServiceCollection ConfigureGithubServices(this IServiceCollection services)
    {
        services.AddSingleton<IGithubSignatureValidator, GithubSignatureValidator>();
        services.AddSingleton<IGithubAppAuthProvider, GithubAppAuthProvider>();
        services.AddScoped<IGithubClientFactory, GithubClientFactory>();
        services.AddScoped<IGithubService, GithubService>();
        services.AddGithubWebhookHandlers();

        return services;
    }

    public static IServiceCollection ConfigureJsonOptions(this IServiceCollection services, Action<JsonOptions> configureOptions)
    {
        services.ConfigureHttpJsonOptions(configureOptions);

        services.Configure<JsonOptions>(Constants.PrettyPrintJsonOptionsKey, options =>
        {
            configureOptions(options);
            options.SerializerOptions.WriteIndented = true;
        });

        services.AddKeyedTransient(KeyedService.AnyKey, (sp, key) =>
        {
            var optionsSnapshot = sp.GetRequiredService<IOptionsSnapshot<JsonOptions>>();
            var jsonOptions = optionsSnapshot.Get(key?.ToString() ?? string.Empty);
            return jsonOptions;
        });

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
        where TEvent : GithubWebhookEvent
        where THandler : GithubWebhookRequestHandler<TEvent>
    {
        services.AddKeyedScoped<IRequestHandler<GithubWebhookEvent>, THandler>(typeof(TEvent));
        return services;
    }


    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
    {
        services.AddHttpContextAccessor();
        services.AddOptions();
        services.ConfigureJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonContext.Default);
        });

        // Enables easy named loggers in static contexts
        services.AddKeyedTransient<ILogger>(KeyedService.AnyKey, (sp, key) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger(key is string keyString ? keyString : "Unknown");
        });

        services.AddOpenApi();

        services.AddTelemetry();

        services.AddHttpClient("Github", client =>
        {
            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HwoodiwissHelper", $"{ApplicationMetadata.Version}+{ApplicationMetadata.GitCommit}"));
        });

        services.AddSingleton(configurationRoot);
        services.AddSingleton<UserAgentBlockMiddleware>();
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponseStatusCode;
            options.RequestHeaders.Add("X-Forwarded-For");
            options.RequestHeaders.Add("X-Real-IP");
        });

        services.ConfigureGithubServices();

        return services;
    }
}
