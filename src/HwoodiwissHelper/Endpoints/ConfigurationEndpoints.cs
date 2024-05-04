using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
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

        group.MapGet("/debug", (IConfiguration config) => config is IConfigurationRoot root
                ? root.GetConfigurationDebug(environment)
                : []);

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
            ["isNativeAot"] = JsonValue.Create(!(RuntimeFeature.IsDynamicCodeSupported & RuntimeFeature.IsDynamicCodeCompiled)),
            ["isKubernetes"] = JsonValue.Create(ApplicationMetadata.IsKubernetes),
        }));

        group.MapGet("/reload", ([FromServices] IConfigurationRoot config) =>
        {
            config.Reload();
        });

        return builder;
    }

    private static JsonObject GetConfigurationDebug(this IConfigurationRoot root, IWebHostEnvironment environment)
    {
        var json = new JsonObject(new() { PropertyNameCaseInsensitive = true });

        string? ToDisplayValue(string? value) => environment.IsDevelopment()
            ? value
            : $"sha512:{Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty)))}";

        void RecurseChildren(
            JsonObject parent,
            IEnumerable<IConfigurationSection> children)
        {
            foreach (var child in children)
            {
                var (value, provider) = root.GetValueAndProvider(child.Path);

                var localParent = parent;

                if (provider != null)
                {
                    parent[child.Key] = new JsonObject()
                    {
                        ["value"] = JsonValue.Create(ToDisplayValue(value)),
                        ["provider"] = JsonValue.Create(provider.ToString()),
                    };
                }
                else if (!string.IsNullOrEmpty(child.Key))
                {
                    parent[child.Key] = localParent = [];
                }

                RecurseChildren(localParent, child.GetChildren());
            }
        }

        RecurseChildren(json, root.GetChildren());

        return json;
    }

    public static (string? Value, IConfigurationProvider? Provider) GetValueAndProvider(this IConfigurationRoot root, string key)
    {
        foreach (var provider in root.Providers)
        {
            if (provider.TryGet(key, out var value))
            {
                return (value, provider);
            }
        }

        return (null, null);
    }
}
