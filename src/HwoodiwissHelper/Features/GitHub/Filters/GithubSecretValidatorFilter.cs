using ArgumentativeFilters;
using HwoodiwissHelper.Features.GitHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace HwoodiwissHelper.Features.GitHub.Filters;

public static partial class GithubSecretValidatorFilter
{
    [ArgumentativeFilter]
    private static async ValueTask<object?> ValidateGithubSecret(
        [FromServices] IGitHubSignatureValidator gitHubSignatureValidator,
        [FromKeyedServices(nameof(GithubSecretValidatorFilter))] ILogger logger,
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-Hub-Signature-256", out var signature)
            || signature.Count is not 1
            || !await gitHubSignatureValidator.ValidateSignatureAsync(signature.ToString().AsMemory()[7..], context.HttpContext.Request.Body, CancellationToken.None))
        {
            Log.SecretValidationFailed(logger, signature.ToString());
            return Results.BadRequest();
        }

        return await next(context);
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Warning, "GitHub secret failed validation {Signature}")]
        public static partial void SecretValidationFailed(ILogger logger, string signature);
    }
}
