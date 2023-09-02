using HwoodiwissHelper.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HwoodiwissHelper.Tests.Integration;

public class HwoodiwissHelperFixture : WebApplicationFactory<Program>
{
    public GithubSignatureValidatorInterceptor SignatureValidator { get; } = new(); 
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(cfg => 
            cfg.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Github:WebhookKey"] = "test_key",
                }
            ));
        
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IGithubSignatureValidator>(SignatureValidator);
        });
    }
}
