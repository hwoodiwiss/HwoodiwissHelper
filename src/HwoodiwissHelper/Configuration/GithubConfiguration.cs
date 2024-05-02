namespace HwoodiwissHelper.Configuration;

public sealed class GithubConfiguration : INamedConfiguration
{
    public static string SectionName => "Github";

    public required string WebhookKey { get; set; }

    public required bool EnableRequestLogging { get; set; }

    public required string AppId { get; set; }

    public required string AppPrivateKey { get; set; }

    public required string[] AllowedBots { get; set; }
}
