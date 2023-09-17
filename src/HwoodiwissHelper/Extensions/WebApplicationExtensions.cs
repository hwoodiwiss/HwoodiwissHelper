using HwoodiwissHelper.Features.Configuration;
using HwoodiwissHelper.Features.Github;
using HwoodiwissHelper.Features.Health;
using HwoodiwissHelper.Features.Surprise;
using HwoodiwissHelper.Middleware;

namespace HwoodiwissHelper.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() && !ApplicationMetadata.IsNativeAot)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.MapEndpoints(app.Environment);
        
        return app;
    }

    private static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder, IWebHostEnvironment environment)
        => builder
            .MapConfigurationEndpoints(environment)
            .MapHealthEndpoints()
            .MapSurpriseEndpoints()
            .MapGithubEndpoints();
}
