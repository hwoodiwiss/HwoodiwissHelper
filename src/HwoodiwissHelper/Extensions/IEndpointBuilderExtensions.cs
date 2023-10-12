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
