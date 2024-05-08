using System.Globalization;
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
            false,
            f.Internet.Email(),
            f.Random.Long(0),
            f.Internet.UserName(),
            f.Person.FullName,
            ActorType.User));

        var branchFaker = new Faker<Branch>().CustomInstantiator(f => new Branch(
            f.Lorem.Sentence(),
            f.Lorem.Sentence(),
            f.Random.Hash(),
            actorFaker.Generate()));

        var repoFaker = new Faker<Repository>().CustomInstantiator(f => new Repository(
            f.Random.Long(0),
            f.Lorem.Sentence(),
            actorFaker.Generate()
        ));

        var pullRequestFaker = new Faker<PullRequestInfo>().CustomInstantiator(f => new PullRequestInfo(
            f.Internet.Url(),
            f.Random.Long(0),
            f.Random.Int(0),
            f.Random.Guid().ToString(),
            actorFaker.Generate(),
            f.Internet.Url(),
            [],
            f.Date.Recent().ToString(CultureInfo.InvariantCulture),
            f.Date.Recent().ToString(CultureInfo.InvariantCulture),
            branchFaker.Generate(),
            branchFaker.Generate(),
            AuthorAssociation.Contributor,
            null,
            false));

        var installationFaker = new Faker<Installation>().CustomInstantiator(f => new Installation(
            f.Random.Int(0),
            f.Random.Hash()));

        TheoryData<string, object> data = new()
        {
            {"workflow_run", CreateTestEvent("completed") },
            {"workflow_run", CreateTestEvent("in_progress") },
            {"workflow_run", CreateTestEvent("requested") },
        };

        return data;
    }

    private static TestWebhookEvent CreateTestEvent(string action) =>
        new()
        {
            Action = action,
            Installation = new Installation(1, "hash"),
            Sender = new Actor(false, "email", 1, "login", "name", ActorType.User)
        };

    private sealed record TestWebhookEvent : GithubWebhookEvent
    {
        [JsonPropertyName("action")]
        public required string Action { get; init; }
    }
}
