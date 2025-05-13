using Microsoft.AspNetCore.Components;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Components;

public sealed partial class GitHubForkList : ComponentBase
{
    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Confirming deletion of {ForkCount} forks.")]
        public static partial void ConfirmForkDeletion(ILogger logger, int forkCount);
    }
}
