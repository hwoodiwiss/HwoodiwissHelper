namespace HwoodiwissHelper.Models.Github;

public sealed record Label(
    int Id,
    string NodeId,
    string Url,
    string Name,
    string Description,
    string Color,
    bool Default);
