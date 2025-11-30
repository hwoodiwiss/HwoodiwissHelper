using Hwoodiwiss.Extensions.Hosting;

namespace HwoodiwissHelper.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static HwoodiwissApplicationBuilder ConfigureApplication(this HwoodiwissApplicationBuilder builder)
    {
        builder.Services.Configure<HwoodiwissApplicationOptions>(builder.Configuration.GetSection("HwoodiwissApplication"));
        builder.Services.ConfigureHttpClients();
        builder.Services.ConfigureServices(builder.Configuration);

        return builder;
    }
}
