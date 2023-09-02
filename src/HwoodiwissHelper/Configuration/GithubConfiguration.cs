namespace HwoodiwissHelper.Configuration;

public sealed class GithubConfiguration : INamedConfiguration
{
    public static string SectionName => "Github";

    public required string WebhookKey { get; set; }
}
