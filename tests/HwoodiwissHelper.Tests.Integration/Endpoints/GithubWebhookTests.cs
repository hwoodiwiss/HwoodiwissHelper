using System.Net;
using System.Text;
using HwoodiwissHelper.Tests.Integration.Extensions;

namespace HwoodiwissHelper.Tests.Integration.Endpoints;

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
    
    [Theory]
    [InlineData("workflow_run", "complete", "HwoodiwissHelper.Events.Github.WorkflowRun+Complete")]
    [InlineData("workflow_run", "in_progress", "HwoodiwissHelper.Events.Github.WorkflowRun+InProgress")]
    [InlineData("workflow_run", "requested", "HwoodiwissHelper.Events.Github.WorkflowRun+Requested")]
    public async Task Post_GithubWebhook_HandlesKnownWebhookEvents(string webhookEvent, string workflowAction, string deserializedType)
    {
        // Arrange
        HttpRequestMessage requestMessage = new(HttpMethod.Post, "/github/webhook");
        requestMessage.Headers.Add("X-Github-Event", webhookEvent);
        requestMessage.Content = new StringContent($"{{\"action\": \"{workflowAction}\", \"test\": \"value\"}}", Encoding.UTF8, "application/json");
        await requestMessage.SignRequestAsync(HwoodiwissHelperFixture.WebhookSigningKey);
        
        // Act
        var response = await _client.SendAsync(requestMessage);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.ShouldBe(deserializedType);
    }
}
