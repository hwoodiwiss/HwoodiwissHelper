namespace HwoodiwissHelper.Features.GitHub.Configuration;

public sealed class GitHubConfiguration
{
    public static string SectionName => "Github";

    public required string WebhookKey { get; set; }

    public required bool EnableRequestLogging { get; set; }
    
    public Dictionary<string, GitHubAppConfiguration> AppConfigurations { get; set; } = new();
    
    public required string DefaultApplicationName { get; set; }
    
    public required string[] AllowedBots { get; set; }

    public required string[] AllowedRedirectHosts { get; set; }
}
