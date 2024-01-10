using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Bogus;
using HwoodiwissHelper.Events.Github;
using HwoodiwissHelper.Models.Github;
using HwoodiwissHelper.Tests.Integration.Extensions;

namespace HwoodiwissHelper.Tests.Integration.Endpoints;

public sealed class GithubWebhookTests(HwoodiwissHelperFixture fixture) : IClassFixture<HwoodiwissHelperFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };
    
    [Theory]
    [MemberData(nameof(WebhookData))]
    public async Task Post_GithubWebhook_HandlesKnownWebhookEvents(string webhookEvent, object webhookData)
    {
        // Arrange
        HttpRequestMessage requestMessage = new(HttpMethod.Post, "/github/webhook");
        requestMessage.Headers.Add("X-Github-Event", webhookEvent);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(webhookData, _jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);
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

    public static TheoryData<string, object> WebhookData()
    {
        var actorFaker = new Faker<Actor>().CustomInstantiator(f => new Actor(
            f.Internet.Url(),
            false,
            f.Internet.Email(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Random.Long(0),
            f.Internet.UserName(),
            f.Person.FullName,
            f.Random.Guid().ToString(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.UserName(),
            f.Internet.Url(),
            f.Internet.Url(),
            ActorType.User,
            f.Internet.Url()));
        
        TheoryData<string, object> data = new()
        {
            {"workflow_run", new TestWebhookEvent("completed", actorFaker.Generate()) },
            {"workflow_run", new TestWebhookEvent("in_progress", actorFaker.Generate()) },
            {"workflow_run", new TestWebhookEvent("requested", actorFaker.Generate()) },
            {"pull_request", new TestWebhookEvent("opened", actorFaker.Generate()) },
        };
        
        return data;
    }
    
    private sealed record TestWebhookEvent([property: JsonPropertyOrder(-1)]string Action, Actor Sender) : GithubWebhookEvent(Sender);
}


