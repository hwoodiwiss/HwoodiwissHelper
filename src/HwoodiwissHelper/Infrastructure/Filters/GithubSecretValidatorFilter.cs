using ArgumentativeFilters;

using Microsoft.AspNetCore.Mvc;

namespace HwoodiwissHelper.Infrastructure.Filters;

public static partial class GithubSecretValidatorFilter
{
    [ArgumentativeFilter]
    private static async ValueTask<object?> ValidateGithubSecret(
        [FromServices] IGithubSignatureValidator githubSignatureValidator,
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-Hub-Signature-256", out var signature) 
            || signature.Count is not 1
            || !await githubSignatureValidator.ValidateSignatureAsync(signature.ToString().AsMemory()[7..], context.HttpContext.Request.Body, CancellationToken.None))
        {
            return Results.BadRequest();
        }
        
        return await next(context);
    }
}
