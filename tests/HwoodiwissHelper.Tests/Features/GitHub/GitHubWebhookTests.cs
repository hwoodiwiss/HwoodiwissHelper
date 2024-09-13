using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using HwoodiwissHelper.Features.GitHub.Events;
using HwoodiwissHelper.Features.GitHub.Events.Models;
using HwoodiwissHelper.Tests.Integration.Extensions;

namespace HwoodiwissHelper.Tests.Integration.Features.Github;

[Collection(IntegrationTestCollection.Name)]
public sealed class GitHubWebhookTests(IntegrationFixture fixture)
{
    private readonly HttpClient _client = fixture.CreateClient();

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    [Theory(Skip = "Needs rethinking")]
    [MemberData(nameof(WebhookData))]
    public async Task Post_GithubWebhook_HandlesKnownWebhookEvents(string webhookEvent, string webhookPayload)
    {
        // Arrange
        HttpRequestMessage requestMessage = new(HttpMethod.Post, "/github/webhook");
        requestMessage.Headers.Add("X-Github-Event", webhookEvent);
        requestMessage.Content = new StringContent(webhookPayload, Encoding.UTF8, MediaTypeNames.Application.Json);
        await requestMessage.SignRequestAsync();

        // Act
        var response = await _client.SendAsync(requestMessage);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Theory(Skip = "Needs rethinking")]
    [InlineData("issue_comment", "edited")]
    public async Task Post_GithubWebhook_GracefullyHandlesUnknownWebhookEvents(string webhookEvent, string workflowAction)
    {
        // Arrange
        HttpRequestMessage requestMessage = new(HttpMethod.Post, "/github/webhook");
        requestMessage.Headers.Add("X-Github-Event", webhookEvent);
        requestMessage.Content = new StringContent($"{{\"action\": \"{workflowAction}\", \"test\": \"value\"}}", Encoding.UTF8, "application/json");
        await requestMessage.SignRequestAsync();

        // Act
        var response = await _client.SendAsync(requestMessage);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    public static TheoryData<string, string> WebhookData()
    {
        TheoryData<string, string> data = new()
        {
            {"pull_request", CreateTestEvent("pull_request_opened")},
        };

        return data;
    }

    private static string CreateTestEvent(string fileName)
    {
        var absolutDataPath = Path.GetFullPath("./Features/Github/Events");
        var eventPath = Path.Combine(absolutDataPath, $"{fileName}.json");
        return File.ReadAllText(eventPath);
    }
}
