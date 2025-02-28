using System.Net.Http.Headers;
using System.Net.Http.Json;
using HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.HttpClients;

public class GitHubClient(HttpClient httpClient)
{
    public async Task<User> GetUserInfo(string authToken)
    {
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/user");
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        using var response = await httpClient.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync(GitHubClientJsonSerializerContext.Default.User) ?? throw new InvalidOperationException();
    }

    public async Task<Repository[]> GetUserForks(string userLogin, string authToken)
    {
        List<Repository> repositories = new(100);
        bool lastPageEmpty = false;
        
        while(lastPageEmpty is false)
        {
            var page = repositories.Count / 100 + 1;
            var pageRepositories = await GetUserForkPage(userLogin, authToken, page);
            if(pageRepositories.Length == 0)
            {
                lastPageEmpty = true;
            }
            else
            {
                repositories.AddRange(pageRepositories);
            }
        }
        
        return repositories.Where(w => w.Fork).ToArray();
    }
    
    private async Task<Repository[]> GetUserForkPage(string userLogin, string authToken, int page)
    {
        var perPage = 100;
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"/users/{userLogin}/repos?per_page={perPage}&page={page}");
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        using var response = await httpClient.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync(GitHubClientJsonSerializerContext.Default.RepositoryArray) ?? [];
    }
}
