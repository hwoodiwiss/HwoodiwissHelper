using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using HwoodiwissHelper.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HwoodiwissHelper.Endpoints;

public static class ConfigurationEndpoints
{
    public static IEndpointRouteBuilder MapConfigurationEndpoints(this IEndpointRouteBuilder builder, IWebHostEnvironment environment)
    {
        var group = builder.MapGroup("/configuration")
            .WithPrettyPrint();

        if (ApplicationMetadata.IsKubernetes || environment.IsDevelopment())
        {
            group.MapGet("/", (IConfiguration config) => config.AsEnumerable().ToDictionary(k => k.Key, v => v.Value));
        }
        
        group.MapGet("/version", () => new Dictionary<string, string>
        {
            ["isNativeAot"] = ApplicationMetadata.IsNativeAot.ToString(CultureInfo.InvariantCulture),
            ["version"] = ApplicationMetadata.Version,
            ["gitBranch"] = ApplicationMetadata.GitBranch,
            ["gitCommit"] = ApplicationMetadata.GitCommit,
            ["systemArchitecture"] = RuntimeInformation.RuntimeIdentifier,
            ["runtimeVersion"] = RuntimeInformation.FrameworkDescription,
            ["aspNetCoreVersion"] = typeof(WebApplication).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown",
            ["aspNetCoreRuntimeVersion"] = typeof(WebApplication).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "Unknown",
        });

        group.MapGet("/reload", ([FromServices]IConfigurationRoot config) =>
        {
            config.Reload();
        });
        
        return builder;
    }
}
