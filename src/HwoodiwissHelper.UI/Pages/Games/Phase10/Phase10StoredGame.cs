namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

internal sealed class Phase10StoredGame
{
    public List<Phase10StoredPlayer> Players { get; set; } = [];
    public List<Phase10Round> Rounds { get; set; } = [];
    public int CurrentRound { get; set; } = 1;
    public bool IsComplete { get; set; }
}
