using System.Runtime.CompilerServices;
using HwoodiwissHelper.Endpoints;
using HwoodiwissHelper.Infrastructure;
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

        app.UseStaticFiles();
        return app;
    }

    private static WebApplication UseStaticFiles(this WebApplication app)
    {
        var opts = new StaticFileOptions
        {
            FileProvider =
                new PrecompressedStaticFileProvider(app.Environment,
                    app.Services.GetRequiredService<IHttpContextAccessor>()),
            OnPrepareResponse = ctx =>
            {
                var filename = ctx.File;
                var contentEncoding = ctx.File.Name[^2..] switch
                {
                    "gz" => "gzip",
                    "br" => "br",
                    _ => null,
                };
                if (contentEncoding is not null)
                {
                    ctx.Context.Response.Headers["Content-Encoding"] = contentEncoding;
                }
            }
        };
        app.UseStaticFiles(opts);

        return app;
    }

    private static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder, IWebHostEnvironment environment)
        => builder
            .MapConfigurationEndpoints(environment)
            .MapHealthEndpoints()
            .MapSurpriseEndpoints()
            .MapGithubEndpoints();
}
