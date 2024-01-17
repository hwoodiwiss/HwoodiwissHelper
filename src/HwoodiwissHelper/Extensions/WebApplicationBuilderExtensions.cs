using System.Net.Http.Headers;
using HwoodiwissHelper.Configuration;
using HwoodiwissHelper.Infrastructure.Github;
using HwoodiwissHelper.Middleware;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
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
    
    private static IConfigurationBuilder ConfigureConfiguration(this IConfigurationBuilder configurationBuilder)
        => configurationBuilder
            .AddJsonFile("appsettings.Secrets.json", true, true);
    
    private static IServiceCollection ConfigureOptionsFor<T>(this IServiceCollection serviceProvider, ConfigurationManager configuration)
        where T : class, INamedConfiguration 
    {
        // TODO: Make this work properly at some point, need to experiment with generic usage discovery, then see if that can be fed back into the source generator
        serviceProvider.Configure<GithubConfiguration>(configuration.GetSection(T.SectionName));
        return serviceProvider;
    }
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
    {
        services.AddOptions();
        services.ConfigureJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonContext.Default);
            options.SerializerOptions.TypeInfoResolverChain.Insert(1, GithubJsonContext.Default);
        });

        // Enables easy named loggers in static contexts
        services.AddKeyedTransient<ILogger>(KeyedService.AnyKey, (sp, key) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger(key as string ?? (key.ToString() ?? "Unknown"));
        });
        
        if (!ApplicationMetadata.IsNativeAot)
        {
            services.AddEndpointsApiExplorer();
            services.AddOpenApiDocument();
        }

        services.AddTelemetry();
        services.AddGithubWebhookHandlers();

        services.AddHttpClient("Github", client =>
        {
            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HwoodiwissHelper", $"{ApplicationMetadata.Version}+{ApplicationMetadata.GitCommit}"));
        });
        
        services.AddSingleton(configurationRoot);
        services.AddSingleton<IGithubSignatureValidator, GithubSignatureValidator>();
        services.AddSingleton<UserAgentBlockMiddleware>();
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponseStatusCode;
            options.RequestHeaders.Add("X-Forwarded-For");
            options.RequestHeaders.Add("X-Real-IP");
        });
        services.AddSingleton<IGithubAppAuthProvider, GithubAppAuthProvider>();
        services.AddScoped<IGithubRequestAdaptor, GithubAppHttpClientRequestAdaptor>();
        
        return services;
    }
    
    private static IServiceCollection ConfigureJsonOptions(this IServiceCollection services, Action<JsonOptions> configureOptions)
    {
        services.ConfigureHttpJsonOptions(configureOptions);

        services.Configure<JsonOptions>(Constants.PrettyPrintJsonOptionsKey, options =>
        {
            configureOptions(options);
            options.SerializerOptions.WriteIndented = true;
        });
        
        services.AddKeyedTransient<JsonOptions>(KeyedService.AnyKey, (sp, key) =>
        {
            var optionsSnapshot = sp.GetRequiredService<IOptionsSnapshot<JsonOptions>>();
            var jsonOptions = optionsSnapshot.Get(key.ToString());
            return jsonOptions;
        });

        return services;
    }
}
