using System.Diagnostics;
using System.Reflection;

namespace HwoodiwissHelper;

public static class ApplicationMetadata
{
    public static string Name => typeof(ApplicationMetadata).Assembly.GetName().Name ?? string.Empty;

    public static string Version => GetVersion();

    public static string GitBranch => GetCustomMetadata("GitBranch");

    public static string GitCommit => GetCustomMetadata("GitCommit");

    public static bool IsKubernetes => Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST") is not null;

    private static string GetVersion() => typeof(ApplicationMetadata).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? throw new UnreachableException();

    private static string GetCustomMetadata(string key) => typeof(ApplicationMetadata).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
        .FirstOrDefault(f => f.Key.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value ?? throw new UnreachableException();
}
