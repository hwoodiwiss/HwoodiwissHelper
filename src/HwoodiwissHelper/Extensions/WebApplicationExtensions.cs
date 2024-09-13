using System.Runtime.CompilerServices;
using HwoodiwissHelper.Endpoints;
using HwoodiwissHelper.Features.GitHub.Endpoints;
using HwoodiwissHelper.Infrastructure;
using HwoodiwissHelper.Middleware;

namespace HwoodiwissHelper.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        app.UseMiddleware<UserAgentBlockMiddleware>();
        app.UseDefaultFiles();

        app.UseHttpLogging();
        app.MapEndpoints(app.Environment);

        app.MapOpenApi();

        if (app.Environment.IsDevelopment())
        {
            app.UseStaticFiles();
        }
        else
        {
            app.MapStaticAssets();
            app.MapFallbackToFile("/", "/index.html");
        }

        return app;
    }

    private static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder, IWebHostEnvironment environment)
        => builder
            .MapConfigurationEndpoints(environment)
            .MapHealthEndpoints()
            .MapSurpriseEndpoints()
            .MapGitHubEndpoints();
}
