namespace HwoodiwissHelper.Infrastructure.Github;

public interface IGithubAppAuthProvider
{
    ValueTask<string> GetGithubJwt();
    
    ValueTask<string?> GetToken();
}
