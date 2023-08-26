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
}
