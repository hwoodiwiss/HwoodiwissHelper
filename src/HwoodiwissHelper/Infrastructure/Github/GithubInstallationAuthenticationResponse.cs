namespace HwoodiwissHelper.Infrastructure.Github;

public class GithubInstallationAuthenticationResponse
{
    public string Token { get; set; } = null!;
    public DateTimeOffset ExpiresAt { get; set; }
    public Dictionary<string, string> Permissions { get; set; } = null!;
    public string RepositorySelection { get; set; } = null!;
}
