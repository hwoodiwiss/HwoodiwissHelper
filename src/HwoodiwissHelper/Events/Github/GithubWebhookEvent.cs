using System.Text.Json.Serialization;
using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper.Events.Github;

public abstract record GithubWebhookEvent(
    [property: JsonPropertyName("sender")]
    Actor Sender,
    [property: JsonPropertyName("installation")]
    Installation Installation);
