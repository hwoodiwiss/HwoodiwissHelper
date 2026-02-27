namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

internal sealed class Phase10RoundEntry
{
    private Phase10RoundEntry() { }

    public string PlayerName { get; init; } = "";
    public int Phase { get; init; }
    public bool PhaseCompleted { get; init; }
    public int PointsAdded { get; init; }

    public static Phase10RoundEntry? TryCreate(string playerName, int phase, bool phaseCompleted, int pointsAdded)
    {
        if (pointsAdded < 0) return null;
        return new Phase10RoundEntry
        {
            PlayerName = playerName,
            Phase = phase,
            PhaseCompleted = phaseCompleted,
            PointsAdded = pointsAdded,
        };
    }
}
