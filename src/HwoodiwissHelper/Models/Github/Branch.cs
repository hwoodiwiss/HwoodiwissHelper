namespace HwoodiwissHelper.Models.Github;

public sealed record Branch(
    string Label,
    string Ref,
    string Sha,
    Actor User);
