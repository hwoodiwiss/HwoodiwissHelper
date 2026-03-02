using Dunet;

namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

[Union]
internal partial record Phase10GameState
{
    public partial record Setup(List<string> PlayerNames);
    public partial record InProgress(Phase10Game Game, RoundEntryModel[] Entries);
    public partial record Complete(Phase10Game Game, Phase10Player.Completed Winner, int CompletedPlayerCount);
}

internal partial record Phase10GameState
{
    public partial record Setup
    {
        public bool CanStart =>
            PlayerNames.Count >= 2 &&
            PlayerNames.All(n => !string.IsNullOrWhiteSpace(n));
    }

    public partial record InProgress
    {
        public IEnumerable<Phase10Player> ActiveStandings =>
            Game.Players
                .OrderByDescending(p => p switch
                {
                    Phase10Player.Active a  => a.Phase,
                    Phase10Player.Completed => 11,
                })
                .ThenBy(p => p switch
                {
                    Phase10Player.Active a   => a.Score,
                    Phase10Player.Completed c => c.Score,
                });
    }

    public partial record Complete
    {
        public IEnumerable<Phase10Player> GameOverStandings =>
            Game.Players
                .OrderByDescending(p => p switch
                {
                    Phase10Player.Completed => 1,
                    Phase10Player.Active    => 0,
                })
                .ThenByDescending(p => p switch
                {
                    Phase10Player.Active a  => a.Phase,
                    Phase10Player.Completed => 10,
                })
                .ThenBy(p => p switch
                {
                    Phase10Player.Active a   => a.Score,
                    Phase10Player.Completed c => c.Score,
                });
    }
}
