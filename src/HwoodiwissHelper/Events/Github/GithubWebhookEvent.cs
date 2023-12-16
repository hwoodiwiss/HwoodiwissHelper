namespace HwoodiwissHelper.Events.Github;

public abstract record GithubWebhookEvent
{
    public string? Action { get; set; }
}
