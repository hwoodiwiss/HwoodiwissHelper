using System.Security.Cryptography;
using System.Text;

namespace HwoodiwissHelper.Tests.Integration.Extensions;

public static class HttpRequestMessageExtensions
{
    public static async Task SignRequestAsync(this HttpRequestMessage message, string key = IntegrationFixture.WebhookSigningKey)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        Stream body;
        if (message.Content is not null)
        {
            body = await message.Content.ReadAsStreamAsync();
        }
        else
        {
            body = Stream.Null;
        }
        var signatureBytes = await HMACSHA256.HashDataAsync(keyBytes, body);
        message.Headers.Add("X-Hub-Signature-256", $"sha256={Convert.ToHexString(signatureBytes)}");
    }
}
