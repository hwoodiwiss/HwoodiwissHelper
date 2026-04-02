namespace HwoodiwissHelper.Features.GitHub.HttpClients;

/// <summary>
/// A delegating handler that short-circuits all outbound GitHub API calls,
/// returning a synthetic 200 OK response. Used when <c>Benchmarks:DryRun</c> is
/// <c>true</c> so that benchmark runs exercise the full request pipeline without
/// making real network calls.
/// </summary>
internal sealed class NullGitHubHttpHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
}
