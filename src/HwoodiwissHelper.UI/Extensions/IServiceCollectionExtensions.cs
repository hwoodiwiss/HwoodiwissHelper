using HwoodiwissHelper.UI.Services;

namespace HwoodiwissHelper.UI.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<ICookieManager, CookieManager>();

        return services;
    }
}
