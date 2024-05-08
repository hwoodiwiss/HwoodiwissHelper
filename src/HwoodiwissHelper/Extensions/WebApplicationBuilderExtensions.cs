using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using HwoodiwissHelper.Middleware;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging.Console;
using OpenTelemetry.Logs;

namespace HwoodiwissHelper.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplication ConfigureAndBuild(this WebApplicationBuilder builder)
    {
        builder.Configuration.ConfigureConfiguration();
        builder.ConfigureLogging(builder.Configuration);
        builder.Services.ConfigureOptions(builder.Configuration);
        builder.Services.ConfigureHttpClients();
        builder.Services.ConfigureServices(builder.Configuration);

        return builder.Build();
    }

    private static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder, ConfigurationManager configuration)
    {
        var loggingBuilder = builder.Logging.AddConfiguration(configuration)
            .AddOpenTelemetry(opt =>
            {
                opt.IncludeScopes = true;
                opt.AddOtlpExporter();
            });

#if DEBUG
        loggingBuilder.AddConsole()
            .AddDebug();

        builder.Services.Configure<ConsoleFormatterOptions>(options =>
        {
            options.IncludeScopes = true;
        });
#endif

        return builder;
    }

    private static IConfigurationBuilder ConfigureConfiguration(this IConfigurationBuilder configurationBuilder) =>
        configurationBuilder
            .AddUserSecrets<Program>();

    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
    {
        services.AddOptions();
        services.ConfigureJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonContext.Default);
        });

        // Enables easy named loggers in static contexts
        services.AddKeyedTransient<ILogger>(KeyedService.AnyKey, (sp, key) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger(key as string ?? (key.ToString() ?? "Unknown"));
        });

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        if (RuntimeFeature.IsDynamicCodeSupported)
        {
            services.AddEndpointsApiExplorer();
            services.AddOpenApiDocument(opt =>
            {
                opt.Title = ApplicationMetadata.Name;
                opt.Version = ApplicationMetadata.Version;
            });
        }

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
