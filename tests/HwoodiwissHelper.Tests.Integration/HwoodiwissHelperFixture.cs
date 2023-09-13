using HwoodiwissHelper.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

namespace HwoodiwissHelper.Tests.Integration;

public class HwoodiwissHelperFixture : WebApplicationFactory<Program>
{
    public const string WebhookSigningKey = "It's a Secret to Everybody";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(cfg => 
            cfg.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Github:WebhookKey"] = WebhookSigningKey,
                }
            ));
        
        base.ConfigureWebHost(builder);
    }
}
