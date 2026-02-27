using HwoodiwissHelper.UI.Services;

namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

internal sealed class Phase10GameService(IAppStateStore store) : IPhase10GameService
{
    private const string StorageKey = "phase10-game";

    private Phase10GameState _state = new Phase10GameState.Setup(["", ""]);

    public Phase10GameState State => _state;

    public async Task LoadGameAsync()
    {
        var stored = await store.GetItemAsync<Phase10StoredGame>(StorageKey);
        if (stored is not null)
            _state = ParseStoredGame(stored);
    }

    public void AddPlayer()
    {
        if (_state is not Phase10GameState.Setup setup) return;
        if (setup.PlayerNames.Count >= 6) return;
        setup.PlayerNames.Add("");
    }

    public void RemovePlayer(int index)
    {
        if (_state is not Phase10GameState.Setup setup) return;
        if (setup.PlayerNames.Count <= 2) return;
        setup.PlayerNames.RemoveAt(index);
    }

    public async Task StartGameAsync()
    {
        if (_state is not Phase10GameState.Setup { CanStart: true } setup) return;

        var game = new Phase10Game
        {
            Players = setup.PlayerNames
                .Select(n => (Phase10Player)new Phase10Player.Active(n.Trim(), 1, 0))
                .ToList()
        };

        _state = new Phase10GameState.InProgress(game, InitEntries(game));
        await SaveGameAsync();
    }

    public async Task SubmitRoundAsync()
    {
        if (_state is not Phase10GameState.InProgress inProgress) return;

        var game = inProgress.Game;
        var entries = inProgress.Entries;
        var round = new Phase10Round { RoundNumber = game.CurrentRound };

        for (var i = 0; i < game.Players.Count; i++)
        {
            if (game.Players[i] is not Phase10Player.Active active) continue;

            var entry = entries[i];
            var points = Math.Max(0, entry.Points);

            round.Entries.Add(Phase10RoundEntry.TryCreate(
                playerName: active.PlayerName,
                phase: active.Phase,
                phaseCompleted: entry.PhaseCompleted,
                pointsAdded: points)!);

            var newScore = active.Score + points;

            game.Players[i] = (entry.PhaseCompleted, active.Phase >= 10) switch
            {
                (true, true)  => new Phase10Player.Completed(active.PlayerName, newScore),
                (true, false) => new Phase10Player.Active(active.PlayerName, active.Phase + 1, newScore),
                _             => new Phase10Player.Active(active.PlayerName, active.Phase, newScore),
            };
        }

        game.Rounds.Add(round);
        game.CurrentRound++;

        var completedPlayers = game.Players
            .Where(p => p is Phase10Player.Completed)
            .Cast<Phase10Player.Completed>()
            .ToList();

        if (completedPlayers.Count > 0)
        {
            var winner = completedPlayers.OrderBy(p => p.Score).First();
            _state = new Phase10GameState.Complete(game, winner, completedPlayers.Count);
        }
        else
        {
            foreach (var entry in entries)
            {
                entry.Points = 0;
                entry.PhaseCompleted = false;
            }
        }

        await SaveGameAsync();
    }

    public async Task AbandonGameAsync()
    {
        ResetToSetup();
        await store.RemoveItemAsync(StorageKey);
    }

    public async Task NewGameAsync()
    {
        ResetToSetup();
        await store.RemoveItemAsync(StorageKey);
    }

    private void ResetToSetup() =>
        _state = new Phase10GameState.Setup(["", ""]);

    private static RoundEntryModel[] InitEntries(Phase10Game game) =>
        game.Players.Select(_ => new RoundEntryModel()).ToArray();

    private async Task SaveGameAsync()
    {
        var stored = _state switch
        {
            Phase10GameState.InProgress inProgress => ToStoredGame(inProgress.Game, isComplete: false),
            Phase10GameState.Complete complete      => ToStoredGame(complete.Game, isComplete: true),
            Phase10GameState.Setup                  => null,
        };

        if (stored is not null)
            await store.SetItemAsync(StorageKey, stored);
    }

    private static Phase10GameState ParseStoredGame(Phase10StoredGame stored)
    {
        var players = stored.Players
            .Select<Phase10StoredPlayer, Phase10Player>(p => p.CompletedGame
                ? new Phase10Player.Completed(p.Name, p.TotalScore)
                : new Phase10Player.Active(p.Name, p.CurrentPhase, p.TotalScore))
            .ToList();

        var game = new Phase10Game
        {
            Players = players,
            Rounds = stored.Rounds,
            CurrentRound = stored.CurrentRound,
        };

        if (stored.IsComplete)
        {
            var winner = players
                .Where(p => p is Phase10Player.Completed)
                .Cast<Phase10Player.Completed>()
                .OrderBy(p => p.Score)
                .First();
            var completedCount = players.Count(p => p is Phase10Player.Completed);
            return new Phase10GameState.Complete(game, winner, completedCount);
        }

        return new Phase10GameState.InProgress(game, InitEntries(game));
    }

    private static Phase10StoredGame ToStoredGame(Phase10Game game, bool isComplete) =>
        new()
        {
            Players = game.Players
                .Select(p => p switch
                {
                    Phase10Player.Active a => new Phase10StoredPlayer
                    {
                        Name = a.PlayerName,
                        CurrentPhase = a.Phase,
                        TotalScore = a.Score,
                        CompletedGame = false,
                    },
                    Phase10Player.Completed c => new Phase10StoredPlayer
                    {
                        Name = c.PlayerName,
                        CurrentPhase = 10,
                        TotalScore = c.Score,
                        CompletedGame = true,
                    },
                })
                .ToList(),
            Rounds = game.Rounds,
            CurrentRound = game.CurrentRound,
            IsComplete = isComplete,
        };
}
