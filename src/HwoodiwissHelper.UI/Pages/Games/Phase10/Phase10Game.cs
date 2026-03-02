namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

internal sealed class Phase10Game
{
    public List<Phase10Player> Players { get; set; } = [];
    public List<Phase10Round> Rounds { get; set; } = [];
    public int CurrentRound { get; set; } = 1;
}
