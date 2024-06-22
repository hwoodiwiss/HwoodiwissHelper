using System.Runtime.CompilerServices;
using HwoodiwissHelper.Endpoints;
using HwoodiwissHelper.Middleware;

namespace HwoodiwissHelper.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        app.Use(UserAgentBlockMiddleware.Middleware);
        app.UseDefaultFiles();

        app.UseBlazorFrameworkFiles();
        app.UseHttpLogging();
        app.MapEndpoints(app.Environment);

        if (RuntimeFeature.IsDynamicCodeSupported)
        {
            app.MapOpenApi();
        }

        app.MapStaticAssets();
        return app;
    }

    private static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder, IWebHostEnvironment environment)
        => builder
            .MapConfigurationEndpoints(environment)
            .MapHealthEndpoints()
            .MapSurpriseEndpoints()
            .MapGithubEndpoints();
}
