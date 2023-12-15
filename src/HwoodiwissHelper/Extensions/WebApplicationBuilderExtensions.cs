using System.Globalization;
using HwoodiwissHelper.Configuration;
using HwoodiwissHelper.Infrastructure;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace HwoodiwissHelper.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplication ConfigureAndBuild(this WebApplicationBuilder builder)
    {
        builder.Configuration.ConfigureConfiguration();
        builder.ConfigureLogging(builder.Configuration, builder.Environment);
        builder.Services.ConfigureOptionsFor<GithubConfiguration>(builder.Configuration);
        builder.Services.ConfigureServices(builder.Configuration);

        return builder.Build();
    }

    private static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder, ConfigurationManager configuration, IHostEnvironment environment)
    {
        builder.Logging.AddConfiguration(configuration)
            .AddConsole();
        
        builder.Services.Configure<ConsoleFormatterOptions>(options =>
        {
            options.IncludeScopes = true;
        });
        
        return builder;
    }
    
    private static IConfigurationBuilder ConfigureConfiguration(this IConfigurationBuilder configurationBuilder)
        => configurationBuilder
            .AddJsonFile("appsettings.Secrets.json", true);
    
    private static IServiceCollection ConfigureOptionsFor<T>(this IServiceCollection serviceProvider, ConfigurationManager configuration)
        where T : class, INamedConfiguration 
    {
        serviceProvider.Configure<GithubConfiguration>(t => configuration.GetSection(T.SectionName).Bind(t));
        return serviceProvider;
    }
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
    {
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
        if (!ApplicationMetadata.IsNativeAot)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        services.AddSingleton(configurationRoot);
        services.AddSingleton<IGithubSignatureValidator, GithubSignatureValidator>();
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponseStatusCode;
            options.RequestHeaders.Add("X-Forwarded-For");
            options.RequestHeaders.Add("X-Real-IP");
        });

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
