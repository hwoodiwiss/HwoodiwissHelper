using System.Diagnostics.CodeAnalysis;

namespace HwoodiwissHelper.Features.GitHub.Configuration;

public sealed class GitHubAppConfiguration
{
    public string AppId { get; set; }
    
    public string PrivateKey { get; set; }
    
    public string ClientId { get; set; }
    
    public string ClientSecret { get; set; }
}
