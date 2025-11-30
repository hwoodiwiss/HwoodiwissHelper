using System.Net;

namespace HwoodiwissHelper.Tests.Integration.Middleware;

[Collection(IntegrationTestCollection.Name)]
public class UserAgentBlockMiddlewareTests(IntegrationFixture fixture)
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Theory]
    [InlineData("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)")]
    [InlineData("TestUa/1.0")]
    public async Task Get_KnownEndpoint_ReturnsNotFound_WhenUserAgentIsBlocked(string userAgent)
    {
        // Arrange
        using var _ = fixture.SetScopedConfiguration("HwoodiwissApplication:BlockedUserAgents:0", userAgent);
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

        // Act
        var response = await _client.GetAsync("/configuration/version");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("Mozilla/5.0 (compatible; BigBot/2.1;)")]
    [InlineData("TestUa/1.0 (compatible; BigBot/2.1;)")]
    public async Task Get_KnownEndpoint_ReturnsNotFound_WhenUserAgentComponentIsBlocked(string userAgent)
    {
        // Arrange
        using var _ = fixture.SetScopedConfiguration("HwoodiwissApplication:BlockedUserAgents:0", "BigBot");
        _client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

        // Act
        var response = await _client.GetAsync("/configuration/version");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_KnownEndpoint_ReturnsOk_WhenUserAgentIsAccepted()
    {
        // Arrange
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0");

        // Act
        var response = await _client.GetAsync("/configuration/version");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
