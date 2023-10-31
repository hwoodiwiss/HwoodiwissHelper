using HwoodiwissHelper.Infrastructure.Filters;

namespace HwoodiwissHelper.Extensions;

public static class IEndpointBuilderExtensions
{
    public static T WithPrettyPrint<T>(this T builder)
        where T : IEndpointConventionBuilder
    {
        builder.AddEndpointFilterFactory(PrettyPrintJson.Factory);

        return builder;
    }

    /// <summary>
    /// Enables fluently defining that an endpoint should use buffered requests on the endpoint builder.
    /// </summary>
    /// <param name="builder">The EndpointConventionBuilder to add buffered requests to.</param>
    /// <typeparam name="T">A concrete typed deriving IEndpointConventionBuilder</typeparam>
    /// <returns><paramref name="builder"/></returns>
    public static T WithBufferedRequest<T>(this T builder)
        where T : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(async (ctx, next) =>
        {
            ctx.HttpContext.Request.EnableBuffering();
            return await next(ctx);
        });

        return builder;
    }
}
