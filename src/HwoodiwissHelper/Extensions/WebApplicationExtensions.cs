using HwoodiwissHelper.Endpoints;
using HwoodiwissHelper.Features.GitHub.Endpoints;
using Hwoodiwiss.Extensions.Hosting;

namespace HwoodiwissHelper.Extensions;

public static class WebApplicationExtensions
{
    public static HwoodiwissApplication ConfigureRequestPipeline(this HwoodiwissApplication app)
    {
        app.MapEndpoints();

        return app;
    }

    private static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
        => builder
            .MapSurpriseEndpoints()
            .MapGitHubEndpoints();
}
