using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        group.MapGet("/version", () => new JsonObject(new Dictionary<string, JsonNode?>()
        {
            ["name"] = JsonValue.Create(ApplicationMetadata.Name),
            ["version"] = JsonValue.Create(ApplicationMetadata.Version),
            ["gitBranch"] = JsonValue.Create(ApplicationMetadata.GitBranch),
            ["gitCommit"] = JsonValue.Create(ApplicationMetadata.GitCommit),
            ["systemArchitecture"] = JsonValue.Create(RuntimeInformation.RuntimeIdentifier),
            ["runtimeVersion"] = JsonValue.Create(RuntimeInformation.FrameworkDescription),
            ["aspNetCoreVersion"] = JsonValue.Create(typeof(WebApplication).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown"),
            ["aspNetCoreRuntimeVersion"] = JsonValue.Create(typeof(WebApplication).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "Unknown"),
            ["isDynamicCodeCompiled"] = JsonValue.Create(RuntimeFeature.IsDynamicCodeCompiled),
            ["isDynamicCodeSupported"] = JsonValue.Create(RuntimeFeature.IsDynamicCodeSupported),
        }));

        group.MapGet("/reload", ([FromServices] IConfigurationRoot config) =>
        {
            config.Reload();
        });

        return builder;
    }
}
