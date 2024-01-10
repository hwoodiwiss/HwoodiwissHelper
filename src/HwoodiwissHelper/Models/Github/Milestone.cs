namespace HwoodiwissHelper.Models.Github;

public record Milestone(
    string Url,
    string HtmlUrl,
    string LabelsUrl,
    int Id,
    string NodeId,
    int Number,
    string State,
    string Title,
    string Description,
    Actor Creator,
    int OpenIssues,
    int ClosedIssues,
    string CreatedAt,
    string UpdatedAt,
    string ClosedAt,
    string DueOn);
