using System.Diagnostics;
using System.Reflection;

namespace HwoodiwissHelper;

public static class ApplicationMetadata
{
    // Can't use const as otherwise we get warnings about unreachable code
    public static bool IsNativeAot =>
#if NativeAot
            true;
#else
            false;
#endif

    public static string Version => typeof(ApplicationMetadata).Assembly.GetName().Version?.ToString() ?? string.Empty;
    
    public static string GitBranch => GetCustomMetadata("GitBranch");
    
    public static string GitCommit => GetCustomMetadata("GitCommit");
    
    private static string GetCustomMetadata(string key) => Assembly.GetEntryAssembly()?.GetCustomAttributes<AssemblyMetadataAttribute>()
        .FirstOrDefault(f => f.Key.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value ?? throw new UnreachableException();
}
