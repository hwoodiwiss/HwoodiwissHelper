using System.Diagnostics;
using System.Security.Cryptography;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Models.Github;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HwoodiwissHelper.Handlers.Github;

public sealed partial class PullRequestOpenedHandler(ILogger<PullRequestOpenedHandler> logger, ActivitySource activitySource) : GithubWebhookRequestHandler<PullRequest.Opened>(logger, activitySource)
{
    protected override ValueTask<object?> HandleGithubEventAsync(PullRequest.Opened request)
    {
        using var activity = ActivitySource.StartActivity("Handling Pull Request Opened Event");
        Actor pullRequestUser = request.PullRequest.User;
        
        activity?.SetTag("pullrequest.number", request.Number);
        activity?.SetTag("pullrequest.user", pullRequestUser.Login);
        
        if (request.PullRequest.User.Type is ActorType.Bot)
        {
            Log.BotPullRequestOpened(logger, pullRequestUser.Name);
        }

        var test = new JsonWebTokenHandler();
        var tokenDesc = new SecurityTokenDescriptor();
        tokenDesc.Claims.Add("test","test");
        var publicRsaKey = File.Open("private.pem", FileMode.Open);
        using var rsa = RSA.Create();
        rsa.ImportFromPem("");
        tokenDesc.SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        test.CreateToken(tokenDesc);
        
        return new ValueTask<object?>(Results.NoContent());
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Bot pull request opened by {UserName}")]
        public static partial void BotPullRequestOpened(ILogger logger, string userName);
    }
}
