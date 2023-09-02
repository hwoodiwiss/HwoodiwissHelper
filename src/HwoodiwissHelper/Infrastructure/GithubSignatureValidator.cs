using System.Buffers;
using System.Security.Cryptography;
using System.Text;

using HwoodiwissHelper.Configuration;

using Microsoft.Extensions.Options;

namespace HwoodiwissHelper.Infrastructure;

public sealed class GithubSignatureValidator(IOptions<GithubConfiguration> githubConfiguration) : IGithubSignatureValidator
{
    private readonly byte[] _keyBytes = Encoding.UTF8.GetBytes(githubConfiguration.Value.WebhookKey);

    public async ValueTask<bool> ValidateSignature(ReadOnlyMemory<char> signature, Stream body, CancellationToken cancellationToken)
    {
        var hasher = new HMACSHA256(_keyBytes);
        var digest = await HashDataAsync(hasher, body, cancellationToken);

        if (digest is null) return false;

        var digestString = Convert.ToHexString(digest);
        return signature.Span.Equals(digestString.AsSpan(), StringComparison.OrdinalIgnoreCase);
    }
    
    private static async ValueTask<byte[]?> HashDataAsync(HMAC hmac, Stream data, CancellationToken cancellationToken)
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
