using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace HwoodiwissHelper.Benchmarks;

[MemoryDiagnoser]
public class HwoodiwissHelperBenchmarks
{
    private BenchmarkWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;
    private StringContent _webhookPayload = null!;
    private string _webhookSignature = null!;

    [GlobalSetup]
    public void Setup()
    {
        _factory = new BenchmarkWebApplicationFactory();
        _client = _factory.CreateClient();

        var payloadJson = ReadEmbeddedPayload("pull_request_opened.json");
        var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);
        var keyBytes = Encoding.UTF8.GetBytes(BenchmarkWebApplicationFactory.WebhookSigningKey);
        var signatureBytes = HMACSHA256.HashData(keyBytes, payloadBytes);
        _webhookSignature = $"sha256={Convert.ToHexString(signatureBytes)}";
        _webhookPayload = new StringContent(payloadJson, Encoding.UTF8, "application/json");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _client.Dispose();
        _factory.Dispose();
        _webhookPayload.Dispose();
    }

    [Benchmark]
    public async Task<HttpResponseMessage> Health()
        => await _client.GetAsync("/health");

    [Benchmark]
    public async Task<HttpResponseMessage> Configuration()
        => await _client.GetAsync("/configuration/version");

    [Benchmark]
    public async Task<HttpResponseMessage> Webhook()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/github/webhook");
        request.Headers.Add("X-Github-Event", "pull_request");
        request.Headers.Add("X-Hub-Signature-256", _webhookSignature);
        request.Content = new StringContent(
            await _webhookPayload.ReadAsStringAsync(),
            Encoding.UTF8,
            "application/json");
        return await _client.SendAsync(request);
    }

    private static string ReadEmbeddedPayload(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .Single(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
