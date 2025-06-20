namespace HwoodiwissHelper.Features.GitHub.Attributes;

public sealed class GitHubApplicationEndpointMetadataAttribute : Attribute, IGitHubApplicationMetadata
{
    public GitHubApplicationEndpointMetadataAttribute(string applicationName)
    {
        ApplicationName = applicationName;
    }
    
    public string ApplicationName { get; set; }
}
