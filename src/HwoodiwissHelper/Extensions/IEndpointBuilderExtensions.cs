using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Extensions;

public static class IEndpointBuilderExtensions
{
    public static T WithPrettyPrint<T>(this T builder)
        where T : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var result = await next(context);
            if (result is {} and not JsonResult)
            {
                var jsonOptionsSnapshot = context.HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<Microsoft.AspNetCore.Http.Json.JsonOptions>>();
                var jsonOptions = jsonOptionsSnapshot.Get(Constants.PrettyPrintJsonOptionsKey);
                var typeInfo = jsonOptions.SerializerOptions.GetTypeInfo(result.GetType());
                return Results.Json(result, typeInfo);
            }
            return result;
        });

        return builder;
    }
}
