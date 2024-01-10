namespace HwoodiwissHelper.Infrastructure.Github;

public interface IGithubAppAuthProvider
{
    Task<string> GetToken();
}
