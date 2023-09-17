namespace HwoodiwissHelper.Middleware;

public sealed partial class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using (CreateRequestLogScope(logger, context))
        {
            Log.Request(logger, context.Request.Path);
            await next(context);
        }
    }

    private static IDisposable? CreateRequestLogScope(ILogger logger, HttpContext context)
    {
        string HeaderValueOrDefault(string headerName, string defaultValue) =>
            context.Request.Headers.TryGetValue(headerName, out var headerValue)
                ? headerValue.ToString()
                : defaultValue;

        return logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestPath"] = context.Request.Path,
            ["RequestIp"] = HeaderValueOrDefault("X-Real-IP", context.Connection.RemoteIpAddress?.ToString() ?? "")
        });
    }
    
    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Request to {RequestPath}")]
        public static partial void Request(ILogger logger, string requestPath);
    }
}
