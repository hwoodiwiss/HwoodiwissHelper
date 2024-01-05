using HwoodiwissHelper.Configuration;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Middleware;

public sealed partial class UserAgentBlockMiddleware : IDisposable
{
    private ApplicationConfiguration _configuration;
    private readonly IDisposable? _configurationSubscription;

    public UserAgentBlockMiddleware(IOptionsMonitor<ApplicationConfiguration> configuration)
    {
        _configuration = configuration.CurrentValue;
        _configurationSubscription = configuration.OnChange(config => _configuration = config);
    }

    private async Task HandleAsync(HttpContext context, RequestDelegate next)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var disallowedUaParts = _configuration.BlockedUserAgents;
        if (disallowedUaParts is not null && ContainsAny(userAgent, disallowedUaParts))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
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
    
    public void Dispose()
    {
        _configurationSubscription?.Dispose();
    }
    
    public static Task Middleware(HttpContext context, RequestDelegate next)
    {
        var middleware = context.RequestServices.GetRequiredService<UserAgentBlockMiddleware>();
        return middleware.HandleAsync(context, next);
    }
    
    private static partial class Log
    {
        
    }
}
