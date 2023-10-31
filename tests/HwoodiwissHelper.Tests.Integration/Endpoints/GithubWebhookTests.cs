using System.Net;
using System.Text;
using HwoodiwissHelper.Tests.Integration.Extensions;

namespace HwoodiwissHelper.Tests.Integration.Endpoints;

public sealed class GithubWebhookTests(HwoodiwissHelperFixture fixture) : IClassFixture<HwoodiwissHelperFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    
    [Theory]
    [InlineData("workflow_run", "complete")]
    [InlineData("workflow_run", "in_progress")]
    [InlineData("workflow_run", "requested")]
    public async Task Post_GithubWebhook_HandlesKnownWebhookEvents(string webhookEvent, string workflowAction)
    {
        // Arrange
        HttpRequestMessage requestMessage = new(HttpMethod.Post, "/github/webhook");
        requestMessage.Headers.Add("X-Github-Event", webhookEvent);
        requestMessage.Content = new StringContent($"{{\"action\": \"{workflowAction}\", \"test\": \"value\"}}", Encoding.UTF8, "application/json");
        await requestMessage.SignRequestAsync(HwoodiwissHelperFixture.WebhookSigningKey);
        
        // Act
        var response = await _client.SendAsync(requestMessage);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    
    [Theory]
    [InlineData("issue_comment", "edited")]
    public async Task Post_GithubWebhook_HandlesUnknownWebhookEvents(string webhookEvent, string workflowAction)
    {
        // Arrange
        HttpRequestMessage requestMessage = new(HttpMethod.Post, "/github/webhook");
        requestMessage.Headers.Add("X-Github-Event", webhookEvent);
        requestMessage.Content = new StringContent($"{{\"action\": \"{workflowAction}\", \"test\": \"value\"}}", Encoding.UTF8, "application/json");
        await requestMessage.SignRequestAsync(HwoodiwissHelperFixture.WebhookSigningKey);
        
        // Act
        var response = await _client.SendAsync(requestMessage);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
