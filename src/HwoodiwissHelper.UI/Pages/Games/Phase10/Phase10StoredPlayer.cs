namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

internal sealed class Phase10StoredPlayer
{
    public string Name { get; set; } = "";
    public int CurrentPhase { get; set; } = 1;
    public int TotalScore { get; set; }
    public bool CompletedGame { get; set; }
}
