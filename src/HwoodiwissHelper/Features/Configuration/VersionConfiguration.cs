using System.Runtime.InteropServices;

namespace HwoodiwissHelper.Features.Configuration;

public sealed record VersionConfiguration(bool IsNativeAot, string Version, string GitBranch, string GitCommit, string SystemArchitecture)
{
    public static VersionConfiguration FromMetadata() => new(ApplicationMetadata.IsNativeAot, ApplicationMetadata.Version, ApplicationMetadata.GitBranch, ApplicationMetadata.GitCommit, RuntimeInformation.RuntimeIdentifier);
};
