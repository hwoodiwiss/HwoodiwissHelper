using System.Text.Json.Serialization;
using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper.Events.Github;

public abstract record GithubWebhookEvent
{
    [JsonPropertyName("sender")]
    public required Actor Sender { get; init; }

    [JsonPropertyName("installation")]
    public required Installation Installation { get; init; }
}
