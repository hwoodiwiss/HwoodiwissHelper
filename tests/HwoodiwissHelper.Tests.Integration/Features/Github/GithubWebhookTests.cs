using System.Net;
using System.Text;

using HwoodiwissHelper.Configuration;
using HwoodiwissHelper.Tests.Integration.Extensions;

using NSubstitute;

namespace HwoodiwissHelper.Tests.Integration.Features.Github;

public sealed class GithubWebhookTests : IClassFixture<HwoodiwissHelperFixture>
{
    private readonly HwoodiwissHelperFixture _fixture;
    private readonly HttpClient _client;

    public GithubWebhookTests(HwoodiwissHelperFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }
    
    [Fact]
    public async Task Post_GithubWebhook_ReturnsOk()
    {
        // Arrange
        HttpRequestMessage requestMessage = new(HttpMethod.Post, "/github/webhook");
        requestMessage.Content = new StringContent("Hello World!", Encoding.UTF8, "text/plain");
        await requestMessage.SignRequestAsync(HwoodiwissHelperFixture.WebhookSigningKey);
        
        // Act
        var response = await _client.SendAsync(requestMessage);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
