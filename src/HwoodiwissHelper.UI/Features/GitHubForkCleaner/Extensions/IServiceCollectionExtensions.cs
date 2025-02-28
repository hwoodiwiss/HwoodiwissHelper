using HwoodiwissHelper.UI.Features.GitHubForkCleaner.HttpClients;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureGitHubServices(this IServiceCollection services)
    {
        services.AddHttpClient<GitHubClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        });

        return services;
    }   
}
