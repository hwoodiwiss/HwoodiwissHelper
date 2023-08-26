using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

using HwoodiwissHelper.Extensions;

namespace HwoodiwissHelper.Features.Configuration;

public static class ConfigurationEndpoints
{
    public static IEndpointRouteBuilder MapConfigurationEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/configuration")
            .WithPrettyPrint();

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
        return builder;
    }
}
