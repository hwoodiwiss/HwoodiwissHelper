using HwoodiwissHelper.Models.Github;

namespace HwoodiwissHelper.Events.Github;

public abstract record GithubWebhookEvent(Actor Sender, Installation Installation);
