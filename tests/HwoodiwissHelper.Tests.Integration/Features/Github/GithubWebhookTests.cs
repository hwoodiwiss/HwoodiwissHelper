using System.Net;
using System.Text;
using HwoodiwissHelper.Tests.Integration.Extensions;

namespace HwoodiwissHelper.Tests.Integration.Features.Github;

public sealed class GithubWebhookTests(HwoodiwissHelperFixture fixture) : IClassFixture<HwoodiwissHelperFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

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
