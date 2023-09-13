using ArgumentativeFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace HwoodiwissHelper.Infrastructure.Filters;

public static partial class PrettyPrintJson
{
    [ArgumentativeFilter]
    private static async ValueTask<object?> PrettyPrintJsonFilter(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
        ,[FromServices] IOptionsSnapshot<JsonOptions> jsonOptionsSnapshot)
        // This will work once we get RC1
        //,[FromKeyedServices(Constants.PrettyPrintJsonOptionsKey)] JsonOptions jsonOptions)
    {
        var result = await next(context);
        if (result is {} and not JsonResult)
        {
            var jsonOptions = jsonOptionsSnapshot.Get(Constants.PrettyPrintJsonOptionsKey);
                
            var typeInfo = jsonOptions.SerializerOptions.GetTypeInfo(result.GetType());
            return Results.Json(result, typeInfo);
        }
        return result;
    }
}
