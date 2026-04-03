extern alias HwoodiwissHelperApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace HwoodiwissHelper.Benchmarks;

public sealed class BenchmarkWebApplicationFactory : WebApplicationFactory<HwoodiwissHelperApp::Program>
{
    public const string WebhookSigningKey = "It's a Secret to Everybody";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Github:WebhookKey"] = WebhookSigningKey,
                ["Github:AppId"] = "1",
                ["Github:AppPrivateKey"] = CreateFakeBase64PrivateKey(),
                ["Github:EnableRequestLogging"] = "false",
                ["Github:AllowedBots:0"] = "dependabot[bot]",
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IHttpMessageHandlerBuilderFilter, StubGitHubHandlerFilter>();
        });

        base.ConfigureWebHost(builder);
    }

    private static string CreateFakeBase64PrivateKey()
    {
        // A minimal PEM-encoded RSA private key placeholder encoded as Base64.
        // The app decodes from Base64 in PostConfigure, so we wrap a dummy PEM string.
        const string fakePem = "-----BEGIN RSA PRIVATE KEY-----\nMIIEowIBAAKCAQEA0Z3VS5JJcds3xHn/ygWep4PAtEsHABPt3AL8kfhIDaF4vHoT\nmDummyKeyMaterialForBenchmarkTestingPurposesOnlyNotARealKey12345678\n-----END RSA PRIVATE KEY-----";
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fakePem));
    }

    private sealed class StubGitHubHandlerFilter : IHttpMessageHandlerBuilderFilter
    {
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return builder =>
            {
                next(builder);
                builder.AdditionalHandlers.Add(new StubHttpMessageHandler());
            };
        }
    }

    private sealed class StubHttpMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
    }
}
