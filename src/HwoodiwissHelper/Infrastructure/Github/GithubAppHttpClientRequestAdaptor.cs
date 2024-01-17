using GitHub.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace HwoodiwissHelper.Infrastructure.Github;

public class GithubAppHttpClientRequestAdaptor(IGithubAppAuthProvider authProvider, HttpClient client)
    : HttpClientRequestAdapter(new TokenAuthenticationProvider(ApplicationMetadata.Name, authProvider.GetGithubJwt()), httpClient: client), 
        IGithubRequestAdaptor;
