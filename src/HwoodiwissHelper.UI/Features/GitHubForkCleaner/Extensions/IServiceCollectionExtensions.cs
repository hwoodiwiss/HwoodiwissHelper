using HwoodiwissHelper.UI.Features.GitHubForkCleaner.Authentication;
using HwoodiwissHelper.UI.Features.GitHubForkCleaner.HttpClients;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureGitHubServices(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddHttpClient<GitHubClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        });
        services.AddSingleton<GitHubAuthentication>();

        return services;
    }   
}
