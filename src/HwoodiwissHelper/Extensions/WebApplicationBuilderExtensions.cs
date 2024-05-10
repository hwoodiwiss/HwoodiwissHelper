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
}
