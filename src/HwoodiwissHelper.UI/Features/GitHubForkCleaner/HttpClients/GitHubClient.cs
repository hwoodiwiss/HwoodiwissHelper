using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using HwoodiwissHelper.Core;
using HwoodiwissHelper.Core.Extensions;
using HwoodiwissHelper.Core.Features.GitHub;
using HwoodiwissHelper.UI.Features.GitHubForkCleaner.Authentication;
using HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.HttpClients;

public class GitHubClient(HttpClient httpClient, GitHubAuthentication gitHubAuthentication)
{
    private const int MaxPageSize = 100;
    
    public async Task<Result<User, GitHubError>> GetUserInfo()
    {
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/user");
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await gitHubAuthentication.GetAccessToken());

        using var response = await httpClient.SendAsync(httpRequestMessage);

        return await HandleGitHubResponse(response, GitHubClientJsonSerializerContext.Default.User);
    }

    public async Task<Result<Repository[], GitHubError>> GetUserForks(string userLogin)
    {
        List<Repository> repositories = new(MaxPageSize);
        bool seenFinalPage = false;
        
        while(seenFinalPage is false)
        {
            var page = repositories.Count / MaxPageSize + 1;

            var result = await GetUserForkPage(userLogin, await gitHubAuthentication.GetAccessToken(), page);
            if (result is Result<Repository[], GitHubError>.Success { Value: var pageRepositories})
            {
                if(pageRepositories.Length < MaxPageSize)
                {
                    seenFinalPage = true;
                }
                
                repositories.AddRange(pageRepositories);
            }
            else
            {
                return result.MapError<Repository[], GitHubError, GitHubError>(x => x switch
                {
                    GitHubError.Unauthorized => new GitHubError.Unauthorized(),
                    GitHubError.UnexpectedResponse { Message: var message, Content: var response } => 
                        new GitHubError.UnexpectedResponse($"Received an unexpected response on page {page} with message {message}", response),
                    _ => new GitHubError.Unknown(),
                });
            }
        }
        
        return repositories.Where(w => w.Fork).ToArray();
    }
    
    private async Task<Result<Repository[], GitHubError>> GetUserForkPage(string userLogin, string authToken, int page)
    {
        var perPage = 100;
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"/users/{userLogin}/repos?per_page={perPage}&page={page}");
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        using var response = await httpClient.SendAsync(httpRequestMessage);

        return await HandleGitHubResponse(response, GitHubClientJsonSerializerContext.Default.RepositoryArray);
    }

    public async Task<Result<T, GitHubError>> HandleGitHubResponse<T>(HttpResponseMessage response, JsonTypeInfo<T> jsonTypeInfo)
    {
        return response.StatusCode switch
        {
            HttpStatusCode.OK or HttpStatusCode.Accepted => await DeserializeResponse(),
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => new GitHubError.Unauthorized(),
            _ => new GitHubError.UnexpectedResponse("GitHub API returned an unexpected status code {response.StatusCode}",
                await response.Content.ReadAsStringAsync())
        };

        async Task<Result<T, GitHubError>> DeserializeResponse()
        {
            try
            {
                var value = await response.Content.ReadFromJsonAsync(jsonTypeInfo);
                if (value is null)
                {
                    return new GitHubError.UnexpectedResponse("GitHub response could not be deserialized", await response.Content.ReadAsStringAsync());
                }

                return value;
            }
            catch (Exception e)
            {
                return new GitHubError.UnexpectedResponse(e.Message, await response.Content.ReadAsStringAsync());
            }
        }
    }
}
