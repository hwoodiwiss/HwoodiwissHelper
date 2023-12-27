using HwoodiwissHelper.Configuration;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Middleware;

public sealed partial class UserAgentRejectionMiddleware(IOptions<ApplicationConfiguration> configuration)
{
    private async Task HandleAsync(HttpContext context, RequestDelegate next)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var disallowedItems = configuration.Value.BlockedUserAgents;
        if (disallowedItems is not null && ContainsAny(userAgent, disallowedItems))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }

    private static bool ContainsAny(string userAgent, string[] disallowedItems)
    {
        foreach (var item in disallowedItems)
        {
            if (userAgent.Contains(item, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
    
    
    public static Task Middleware(HttpContext context, RequestDelegate next)
    {
        var middleware = context.RequestServices.GetRequiredService<UserAgentRejectionMiddleware>();
        return middleware.HandleAsync(context, next);
    }
    
    private static partial class Log
    {
        
    }
}
