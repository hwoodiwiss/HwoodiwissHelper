using JustEat.HttpClientInterception;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace HwoodiwissHelper.Tests.Integration;

public class HwoodiwissHelperFixture : WebApplicationFactory<Program>
{
    public HwoodiwissHelperFixture()
    {
        Environment.SetEnvironmentVariable("OTEL_SDK_DISABLED", "true");
    }
    
    public const string WebhookSigningKey = "It's a Secret to Everybody";

    private readonly Dictionary<string, string?> _configuration = new();
    private IConfigurationRoot? _configurationRoot;
    public HttpClientInterceptorOptions Interceptor { get; } = new();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(
                new Dictionary<string, string?> {["Github:WebhookKey"] = WebhookSigningKey,}
            ).Add(new TestConfigurationSource(_configuration));
            _configurationRoot = cfg as IConfigurationRoot;
        });

        builder.ConfigureLogging(loggingBuilder => 
            loggingBuilder
                .AddConsole()
                .AddDebug()
            ).ConfigureServices(services =>
        {
            services.AddSingleton<IHttpMessageHandlerBuilderFilter, HttpClientInterceptionFilter>(
                (_) => new HttpClientInterceptionFilter(Interceptor));
        });
        
        base.ConfigureWebHost(builder);
    }

    public IDisposable SetScopedConfiguration(string key, string? value)
    {
        _ = _configuration.TryGetValue(key, out string? originalValue);
        _configuration[key] = value;
        _configurationRoot?.Reload();
        return new ConfigurationScope(key, originalValue, _configuration, _configurationRoot);
    }
    
    private sealed class TestConfigurationSource(Dictionary<string, string?> configuration) : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new TestConfigurationProvider(configuration);

        private sealed class TestConfigurationProvider : ConfigurationProvider
        {
            public TestConfigurationProvider(Dictionary<string, string?> configuration)
            {
                Data = configuration;
            }
        }
    }
    
    private sealed class ConfigurationScope(string key, string? originalValue, Dictionary<string, string?> configuration, IConfigurationRoot? configurationRoot) : IDisposable
    {
        public void Dispose()
        {
            if (originalValue is not null)
            {
                configuration[key] = originalValue;
            }
            else
            {
                configuration.Remove(key);
            }
            
            configurationRoot?.Reload();
        }
    }
    
    public sealed class HttpClientInterceptionFilter(HttpClientInterceptorOptions options) : IHttpMessageHandlerBuilderFilter
    {
        /// <inheritdoc/>
        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return (builder) =>
            {
                // Run any actions the application has configured for itself
                next(builder);

                // Add the interceptor as the last message handler
                builder.AdditionalHandlers.Add(options.CreateHttpMessageHandler());
            };
        }
    }
}
