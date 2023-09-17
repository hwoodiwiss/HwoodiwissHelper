using System.Globalization;
using HwoodiwissHelper.Configuration;
using HwoodiwissHelper.Infrastructure;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

namespace HwoodiwissHelper.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplication ConfigureAndBuild(this WebApplicationBuilder builder)
    {
        builder.Configuration.ConfigureConfiguration();
        builder.Host.ConfigureLogging(builder.Configuration, builder.Environment);
        builder.Services.ConfigureOptionsFor<GithubConfiguration>(builder.Configuration);
        builder.Services.ConfigureServices();

        return builder.Build();
    }

    private static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder, ConfigurationManager configuration, IHostEnvironment environment)
    {
        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext();
        if (environment.IsDevelopment())
        {
            loggerConfig
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
        }
        else
        {
            loggerConfig
                .WriteTo.Console(new JsonFormatter())
                .WriteTo.File(new CompactJsonFormatter(), "./logs/application.log", rollingInterval: RollingInterval.Day);
        }

        Log.Logger = loggerConfig.CreateLogger();
        
        hostBuilder.UseSerilog();
        return hostBuilder;
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
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonContext.Default);
        });
        
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        if (!ApplicationMetadata.IsNativeAot)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        services.AddSingleton<IGithubSignatureValidator, GithubSignatureValidator>();

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
