using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

public sealed record PullRequestInfo(
    [property: JsonPropertyName("url")]
    string Url,
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("number")]
    int Number,
    [property: JsonPropertyName("title")]
    string Title,
    [property: JsonPropertyName("user")]
    Actor User,
    [property: JsonPropertyName("body")]
    string? Body,
    [property: JsonPropertyName("labels")]
    Label[] Labels,
    [property: JsonPropertyName("created_at")]
    string CreatedAt,
    [property: JsonPropertyName("updated_at")]
    string UpdatedAt,
    [property: JsonPropertyName("head")]
    Branch Head,
    [property: JsonPropertyName("base")]
    Branch Base,
    [property: JsonPropertyName("author_association")]
    AuthorAssociation AuthorAssociation,
    [property: JsonPropertyName("auto_merge")]
    AutoMerge? AutoMerge,
    [property: JsonPropertyName("draft")]
    bool? Draft);
