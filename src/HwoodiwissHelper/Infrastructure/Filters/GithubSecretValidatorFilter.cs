using System.Buffers;
using System.Security.Cryptography;
using System.Text;

using ArgumentativeFilters;

using HwoodiwissHelper.Configuration;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Infrastructure.Filters;

public static partial class GithubSecretValidatorFilter
{
    [ArgumentativeFilter]
    private static async ValueTask<object?> ValidateGithubSecret(
        [FromServices] IOptions<GithubConfiguration> githubConfiguration,
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-Hub-Signature-256", out var signature) || signature.Count is not 1)
        {
            return Results.BadRequest();
        }
        
        var githubKey = githubConfiguration.Value.WebhookKey;

        var keyBytes = Encoding.UTF8.GetBytes(githubKey);

        var hasher = new HMACSHA256(keyBytes);
        var digest = await hasher.HashDataAsync(context.HttpContext.Request.Body, CancellationToken.None);

        if (!Equals(signature.ToString(), digest))
        {
            return Results.BadRequest();
        }
        
        return await next(context);
    }
    
    private static async Task<byte[]?> HashDataAsync(this HMACSHA256 hmac, Stream data, CancellationToken cancellationToken)
    {
        // Read the body 4096 bytes at a time.
        var buffer = ArrayPool<byte>.Shared.Rent(4096);
        try
        {
            int bytesRead;
            while ((bytesRead = await data.ReadAsync(buffer, cancellationToken)) > 0)
            {
                hmac.TransformBlock(buffer, 0, bytesRead, null, 0);
            }

            hmac.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            return hmac.Hash;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            if (data.CanSeek)
            {
                data.Position = 0;
            }
        }
    }

}
