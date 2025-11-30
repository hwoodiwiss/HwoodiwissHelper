using System.Net.Http.Headers;
using HwoodiwissHelper.Configuration;
using HwoodiwissHelper.Features.GitHub.Extension;

namespace HwoodiwissHelper.Extensions;

public static class IServiceCollectionExtensions
{
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


    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
    {
        services.AddMemoryCache();
        services.ConfigureGitHubServices(configurationRoot);

        return services;
    }
}
