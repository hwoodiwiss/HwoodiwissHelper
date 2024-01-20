using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            f.Random.Bool(),
            f.Internet.Url(),
            f.Internet.Url(),
            ActorType.User,
            f.Internet.Url()));

        var branchFaker = new Faker<Branch>().CustomInstantiator(f => new Branch(
            f.Lorem.Sentence(),
            f.Lorem.Sentence(),
            f.Random.Hash(),
            actorFaker.Generate()));

        var pullRequestFaker = new Faker<PullRequestInfo>().CustomInstantiator(f => new PullRequestInfo(
            f.Internet.Url(),
            f.Random.Long(0),
            f.Random.Guid().ToString(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Internet.Url(),
            f.Random.Int(0),
            "open",
            f.Random.Bool(),
            f.Lorem.Sentence(),
            actorFaker.Generate(),
            f.Lorem.Sentence(),
            Array.Empty<Label>(),
            null,
            null,
            DateTimeOffset.UtcNow.ToString("O"),
            DateTimeOffset.UtcNow.ToString("O"),
            null,
            null,
            null,
            branchFaker.Generate(),
            branchFaker.Generate(),
            AuthorAssociation.Owner,
            null,
            null,
            false,
            null,
            null,
            "unknown",
            null,
            0,
            0,
            true,
            1,
            1,
            1,
            1));

        var installationFaker = new Faker<Installation>().CustomInstantiator(f => new Installation(
            f.Random.Long(0),
            f.Random.Hash()));



        TheoryData<string, object> data = new()
        {
            {"workflow_run", new TestWebhookEvent("completed", actorFaker.Generate(), installationFaker.Generate()) },
            {"workflow_run", new TestWebhookEvent("in_progress", actorFaker.Generate(), installationFaker.Generate()) },
            {"workflow_run", new TestWebhookEvent("requested", actorFaker.Generate(), installationFaker.Generate()) },
            {"pull_request", new PullRequest.Opened(1, pullRequestFaker.Generate(), actorFaker.Generate(), installationFaker.Generate()) },
        };

        return data;
    }

    private sealed record TestWebhookEvent([property: JsonPropertyOrder(-1)] string Action, Actor Sender, Installation Installation) : GithubWebhookEvent(Sender, Installation);
}


