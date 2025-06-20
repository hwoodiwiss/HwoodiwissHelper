using HwoodiwissHelper.Features.GitHub.Attributes;

namespace HwoodiwissHelper.Features.GitHub.Extension;

public static class GitHubApplicationEndpointConventionExtensions
{
    private static readonly GitHubApplicationEndpointMetadataAttribute s_ForkCleanerApplicationAttribute = new ("GitHubForkCleaner");
    
    public static TBuilder ForForkCleaner<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        ref TBuilder local = ref builder;
        local.Add(b => b.Metadata.Add(s_ForkCleanerApplicationAttribute));
        return builder;
    }
    
    public static TBuilder ForGitHubApplication<TBuilder>(this TBuilder builder, string applicationName) where TBuilder : IEndpointConventionBuilder
    {
        ref TBuilder local = ref builder;
        local.Add(b => b.Metadata.Add(new GitHubApplicationEndpointMetadataAttribute(applicationName)));
        return builder;
    }
}
