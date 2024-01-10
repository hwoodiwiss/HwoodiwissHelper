namespace HwoodiwissHelper.Models.Github;

public sealed record Actor(
    string AvatarUrl,
    bool Deleted,
    string? Email,
    string EventsUrl,
    string FollowersUrl,
    string FollowingUrl,
    string GistsUrl,
    string GravatarUrl,
    string HtmlUrl,
    long Id,
    string Login,
    string Name,
    string NodeId,
    string OrganizationsUrl,
    string ReceivedEventsUrl,
    string ReposUrl,
    bool SiteAdmin,
    string StarredUrl,
    string SubscriptionsUrl,
    ActorType Type,
    string Url
    );
