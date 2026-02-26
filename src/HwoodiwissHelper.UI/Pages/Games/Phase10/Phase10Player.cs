using Dunet;

namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

[Union]
internal partial record Phase10Player
{
    public partial record Active(string PlayerName, int Phase, int Score);
    public partial record Completed(string PlayerName, int Score);
}

internal partial record Phase10Player
{
    public string Name => this switch
    {
        Active a    => a.PlayerName,
        Completed c => c.PlayerName,
        _           => throw new InvalidOperationException("Unknown Phase10Player variant"),
    };

    public int TotalScore => this switch
    {
        Active a    => a.Score,
        Completed c => c.Score,
        _           => throw new InvalidOperationException("Unknown Phase10Player variant"),
    };
}
