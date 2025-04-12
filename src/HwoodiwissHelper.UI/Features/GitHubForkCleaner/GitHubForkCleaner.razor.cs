namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner;

public sealed partial class GitHubForkCleaner
{
    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "GitHub authentication state: {IsAuthenticated}")]
        public static partial void LogGitHubAuthenticationState(ILogger logger, bool isAuthenticated);
    }
}
