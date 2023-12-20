using ArgumentativeFilters;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace HwoodiwissHelper.Infrastructure.Filters;

public static partial class PrettyPrintJson
{
    [ArgumentativeFilter]
    private static async ValueTask<object?> PrettyPrintJsonFilter(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next,
        [FromKeyedServices(Constants.PrettyPrintJsonOptionsKey)] JsonOptions jsonOptions)
    {
        var result = await next(context);
        if (result is {} and not IResult)
        {
            var typeInfo = jsonOptions.SerializerOptions.GetTypeInfo(result.GetType());
            return Results.Json(result, typeInfo);
        }
        return result;
    }
}
