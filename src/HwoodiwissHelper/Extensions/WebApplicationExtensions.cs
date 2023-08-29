using HwoodiwissHelper.Features.Configuration;
using HwoodiwissHelper.Features.Health;
using HwoodiwissHelper.Features.Surprise;

namespace HwoodiwissHelper.Extensions;

public static class WebApplicationExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder, IWebHostEnvironment environment)
        => builder
            .MapConfigurationEndpoints(environment)
            .MapHealthEndpoints()
            .MapSurpriseEndpoints();
}
