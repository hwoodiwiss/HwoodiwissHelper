using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

public sealed record SubmitReviewRequest
{
    [JsonPropertyName("body")]
    public required string Body { get; init; }
    
    [JsonPropertyName("event")]
    public SubmitReviewEvent Event { get; init; }
}

[JsonConverter(typeof(JsonStringEnumConverter<SubmitReviewEvent>))]
public enum SubmitReviewEvent
{
    [JsonStringEnumMemberName("COMMENT")]
    Comment,
    [JsonStringEnumMemberName("REQUEST_CHANGES")]
    RequestChanges,
    [JsonStringEnumMemberName("APPROVE")]
    Approve
}
