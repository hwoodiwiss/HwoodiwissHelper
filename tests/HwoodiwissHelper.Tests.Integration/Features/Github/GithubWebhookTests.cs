using System.Net;
using System.Text;

using HwoodiwissHelper.Configuration;

namespace HwoodiwissHelper.Tests.Integration.Features.Github;

public sealed class GithubWebhookTests : IClassFixture<HwoodiwissHelperFixture>
{
    private readonly HttpClient _client;

    public GithubWebhookTests(HwoodiwissHelperFixture fixture)
    {
        _client = fixture.CreateClient();
    }
    
    [Fact]
    public async Task Post_GithubWebhook_ReturnsOk()
    {
        // Arrange
        HttpRequestMessage requestMessage = new(HttpMethod.Post, "/github/webhook");
        requestMessage.Headers.Add("X-Hub-Signature-256", $"test_key");
        
        // Act
        var response = await _client.SendAsync(requestMessage);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
