using HwoodiwissHelper.Configuration;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Middleware;

public sealed partial class UserAgentBlockMiddleware : IMiddleware
{
    private readonly ILogger<UserAgentBlockMiddleware> _logger;
    private ApplicationConfiguration _configuration;
    private readonly IDisposable? _configurationSubscription;

    public UserAgentBlockMiddleware(ILogger<UserAgentBlockMiddleware> logger, IOptionsMonitor<ApplicationConfiguration> configuration)
    {
        _logger = logger;
        _configuration = configuration.CurrentValue;
        _configurationSubscription = configuration.OnChange(config => _configuration = config);
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var disallowedUaParts = _configuration.BlockedUserAgents;
        if (disallowedUaParts is not null && ContainsAny(userAgent, disallowedUaParts))
        {
            Log.BlockedUserAgent(_logger, userAgent);
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

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Blocked request for user agent: {UserAgent}")]
        public static partial void BlockedUserAgent(ILogger logger, string userAgent);
    }
}
