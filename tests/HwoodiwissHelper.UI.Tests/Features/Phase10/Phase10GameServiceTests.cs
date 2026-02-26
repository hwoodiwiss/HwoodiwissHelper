using HwoodiwissHelper.UI.Pages.Games.Phase10;
using HwoodiwissHelper.UI.Services;
using NSubstitute;

namespace HwoodiwissHelper.UI.Tests.Features.Phase10;

public class Phase10GameServiceTests
{
    private readonly IAppStateStore _store = Substitute.For<IAppStateStore>();
    private readonly Phase10GameService _sut;

    // Helpers to extract typed state without repetition
    private Phase10GameState.Setup Setup => (Phase10GameState.Setup)_sut.State;
    private Phase10GameState.InProgress InProgress => (Phase10GameState.InProgress)_sut.State;
    private Phase10GameState.Complete Complete => (Phase10GameState.Complete)_sut.State;

    public Phase10GameServiceTests()
    {
        _store.GetItemAsync<Phase10StoredGame>(Arg.Any<string>()).Returns((Phase10StoredGame?)null);
        _sut = new Phase10GameService(_store);
    }

    // ── CanStart ───────────────────────────────────────────────────────────────

    [Fact]
    public void CanStart_WithTwoValidNames_ReturnsTrue()
    {
        Setup.PlayerNames[0] = "Alice";
        Setup.PlayerNames[1] = "Bob";

        Setup.CanStart.ShouldBeTrue();
    }

    [Fact]
    public void CanStart_WithEmptyName_ReturnsFalse()
    {
        Setup.PlayerNames[0] = "Alice";
        Setup.PlayerNames[1] = "";

        Setup.CanStart.ShouldBeFalse();
    }

    [Fact]
    public void CanStart_WithWhitespaceName_ReturnsFalse()
    {
        Setup.PlayerNames[0] = "Alice";
        Setup.PlayerNames[1] = "   ";

        Setup.CanStart.ShouldBeFalse();
    }

    [Fact]
    public void CanStart_WithOnlyOnePlayer_ReturnsFalse()
    {
        Setup.PlayerNames.RemoveAt(1);

        Setup.CanStart.ShouldBeFalse();
    }

    // ── AddPlayer / RemovePlayer ───────────────────────────────────────────────

    [Fact]
    public void AddPlayer_WhenBelowMaximum_AddsEmptySlot()
    {
        var initialCount = Setup.PlayerNames.Count;

        _sut.AddPlayer();

        Setup.PlayerNames.Count.ShouldBe(initialCount + 1);
        Setup.PlayerNames.Last().ShouldBe("");
    }

    [Fact]
    public void AddPlayer_WhenAtMaximumOfSix_DoesNotAdd()
    {
        while (Setup.PlayerNames.Count < 6)
            _sut.AddPlayer();

        _sut.AddPlayer();

        Setup.PlayerNames.Count.ShouldBe(6);
    }

    [Fact]
    public void RemovePlayer_WhenAboveMinimum_RemovesAtIndex()
    {
        Setup.PlayerNames[0] = "Alice";
        Setup.PlayerNames[1] = "Bob";
        _sut.AddPlayer();
        Setup.PlayerNames[2] = "Charlie";

        _sut.RemovePlayer(1);

        Setup.PlayerNames.Count.ShouldBe(2);
        Setup.PlayerNames[0].ShouldBe("Alice");
        Setup.PlayerNames[1].ShouldBe("Charlie");
    }

    [Fact]
    public void RemovePlayer_WhenAtMinimumOfTwo_DoesNotRemove()
    {
        Setup.PlayerNames[0] = "Alice";
        Setup.PlayerNames[1] = "Bob";

        _sut.RemovePlayer(0);

        Setup.PlayerNames.Count.ShouldBe(2);
    }

    // ── StartGameAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task StartGameAsync_CreatesGameWithTrimmedPlayerNames()
    {
        Setup.PlayerNames[0] = "  Alice  ";
        Setup.PlayerNames[1] = "Bob";

        await _sut.StartGameAsync();

        _sut.State.ShouldBeOfType<Phase10GameState.InProgress>();
        InProgress.Game.Players.Count.ShouldBe(2);
        InProgress.Game.Players[0].Name.ShouldBe("Alice");
        InProgress.Game.Players[1].Name.ShouldBe("Bob");
    }

    [Fact]
    public async Task StartGameAsync_InitialisesAllPlayersAtPhaseOne()
    {
        Setup.PlayerNames[0] = "Alice";
        Setup.PlayerNames[1] = "Bob";

        await _sut.StartGameAsync();

        InProgress.Game.Players
            .Cast<Phase10Player.Active>()
            .ShouldAllBe(a => a.Phase == 1);
    }

    [Fact]
    public async Task StartGameAsync_InitialisesRoundEntries()
    {
        Setup.PlayerNames[0] = "Alice";
        Setup.PlayerNames[1] = "Bob";

        await _sut.StartGameAsync();

        InProgress.Entries.Length.ShouldBe(2);
    }

    [Fact]
    public async Task StartGameAsync_SavesGameToStore()
    {
        Setup.PlayerNames[0] = "Alice";
        Setup.PlayerNames[1] = "Bob";

        await _sut.StartGameAsync();

        await _store.Received(1).SetItemAsync(Arg.Any<string>(), Arg.Any<Phase10StoredGame>());
    }

    [Fact]
    public async Task StartGameAsync_WhenCannotStart_DoesNotTransitionState()
    {
        // Both names are empty (default state)
        await _sut.StartGameAsync();

        _sut.State.ShouldBeOfType<Phase10GameState.Setup>();
        await _store.DidNotReceive().SetItemAsync(Arg.Any<string>(), Arg.Any<Phase10StoredGame>());
    }

    // ── SubmitRoundAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task SubmitRoundAsync_AddsPointsToPlayerTotalScore()
    {
        await StartGameWithPlayers("Alice", "Bob");
        InProgress.Entries[0].Points = 15;
        InProgress.Entries[1].Points = 5;

        await _sut.SubmitRoundAsync();

        InProgress.Game.Players[0].TotalScore.ShouldBe(15);
        InProgress.Game.Players[1].TotalScore.ShouldBe(5);
    }

    [Fact]
    public async Task SubmitRoundAsync_WhenPhaseCompleted_AdvancesPlayerPhase()
    {
        await StartGameWithPlayers("Alice", "Bob");
        InProgress.Entries[0].PhaseCompleted = true;
        InProgress.Entries[1].PhaseCompleted = false;

        await _sut.SubmitRoundAsync();

        var alice = InProgress.Game.Players[0].ShouldBeOfType<Phase10Player.Active>();
        alice.Phase.ShouldBe(2);
        var bob = InProgress.Game.Players[1].ShouldBeOfType<Phase10Player.Active>();
        bob.Phase.ShouldBe(1);
    }

    [Fact]
    public async Task SubmitRoundAsync_WhenPhaseNotCompleted_PhaseStaysTheSame()
    {
        await StartGameWithPlayers("Alice", "Bob");
        InProgress.Entries[0].PhaseCompleted = false;

        await _sut.SubmitRoundAsync();

        var alice = InProgress.Game.Players[0].ShouldBeOfType<Phase10Player.Active>();
        alice.Phase.ShouldBe(1);
    }

    [Fact]
    public async Task SubmitRoundAsync_WhenPlayerCompletesPhase10_TransitionsToComplete()
    {
        await StartGameWithPlayers("Alice", "Bob");
        // Force Alice to phase 10 by replacing the player instance
        InProgress.Game.Players[0] = new Phase10Player.Active("Alice", 10, 0);
        InProgress.Entries[0].PhaseCompleted = true;

        await _sut.SubmitRoundAsync();

        _sut.State.ShouldBeOfType<Phase10GameState.Complete>();
        Complete.Winner.PlayerName.ShouldBe("Alice");
    }

    [Fact]
    public async Task SubmitRoundAsync_AddsRoundToHistory()
    {
        await StartGameWithPlayers("Alice", "Bob");

        await _sut.SubmitRoundAsync();

        InProgress.Game.Rounds.Count.ShouldBe(1);
        InProgress.Game.Rounds[0].RoundNumber.ShouldBe(1);
    }

    [Fact]
    public async Task SubmitRoundAsync_IncrementsCurrentRound()
    {
        await StartGameWithPlayers("Alice", "Bob");

        await _sut.SubmitRoundAsync();

        InProgress.Game.CurrentRound.ShouldBe(2);
    }

    [Fact]
    public async Task SubmitRoundAsync_ResetsRoundEntryPointsAndPhaseFlag()
    {
        await StartGameWithPlayers("Alice", "Bob");
        InProgress.Entries[0].Points = 25;
        InProgress.Entries[0].PhaseCompleted = true;

        await _sut.SubmitRoundAsync();

        InProgress.Entries[0].Points.ShouldBe(0);
        InProgress.Entries[0].PhaseCompleted.ShouldBeFalse();
    }

    // ── AbandonGame / NewGame ─────────────────────────────────────────────────

    [Fact]
    public async Task AbandonGameAsync_ResetsToSetupState()
    {
        await StartGameWithPlayers("Alice", "Bob");

        await _sut.AbandonGameAsync();

        _sut.State.ShouldBeOfType<Phase10GameState.Setup>();
        Setup.PlayerNames.Count.ShouldBe(2);
    }

    [Fact]
    public async Task AbandonGameAsync_RemovesGameFromStore()
    {
        await StartGameWithPlayers("Alice", "Bob");

        await _sut.AbandonGameAsync();

        await _store.Received(1).RemoveItemAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task NewGameAsync_ResetsToSetupState()
    {
        await StartGameWithPlayers("Alice", "Bob");

        await _sut.NewGameAsync();

        _sut.State.ShouldBeOfType<Phase10GameState.Setup>();
        Setup.PlayerNames.Count.ShouldBe(2);
    }

    // ── LoadGameAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task LoadGameAsync_WhenStoreHasInProgressGame_RestoresInProgressState()
    {
        var savedGame = new Phase10StoredGame
        {
            Players =
            [
                new Phase10StoredPlayer { Name = "Alice", CurrentPhase = 2, TotalScore = 10 },
                new Phase10StoredPlayer { Name = "Bob",   CurrentPhase = 1, TotalScore = 5  },
            ],
            CurrentRound = 3,
            IsComplete = false,
        };
        _store.GetItemAsync<Phase10StoredGame>(Arg.Any<string>()).Returns(savedGame);

        await _sut.LoadGameAsync();

        _sut.State.ShouldBeOfType<Phase10GameState.InProgress>();
        InProgress.Game.CurrentRound.ShouldBe(3);
        InProgress.Entries.Length.ShouldBe(2);
    }

    [Fact]
    public async Task LoadGameAsync_WhenStoreIsEmpty_RemainsInSetupState()
    {
        _store.GetItemAsync<Phase10StoredGame>(Arg.Any<string>()).Returns((Phase10StoredGame?)null);

        await _sut.LoadGameAsync();

        _sut.State.ShouldBeOfType<Phase10GameState.Setup>();
    }

    [Fact]
    public async Task LoadGameAsync_WhenStoreHasCompleteGame_RestoresCompleteState()
    {
        var savedGame = new Phase10StoredGame
        {
            Players =
            [
                new Phase10StoredPlayer { Name = "Alice", CurrentPhase = 10, TotalScore = 20, CompletedGame = true },
                new Phase10StoredPlayer { Name = "Bob",   CurrentPhase = 3,  TotalScore = 10 },
            ],
            CurrentRound = 5,
            IsComplete = true,
        };
        _store.GetItemAsync<Phase10StoredGame>(Arg.Any<string>()).Returns(savedGame);

        await _sut.LoadGameAsync();

        _sut.State.ShouldBeOfType<Phase10GameState.Complete>();
        Complete.Winner.PlayerName.ShouldBe("Alice");
    }

    // ── Computed properties ───────────────────────────────────────────────────

    [Fact]
    public async Task GameWinner_IsPlayerWithLowestScoreAmongThoseWhoCompletedGame()
    {
        await StartGameWithPlayers("Alice", "Bob");
        // Force both to phase 10 then complete the round so both finish
        InProgress.Game.Players[0] = new Phase10Player.Active("Alice", 10, 0);
        InProgress.Game.Players[1] = new Phase10Player.Active("Bob",   10, 0);
        InProgress.Entries[0].PhaseCompleted = true;
        InProgress.Entries[0].Points = 50;
        InProgress.Entries[1].PhaseCompleted = true;
        InProgress.Entries[1].Points = 30;

        await _sut.SubmitRoundAsync();

        _sut.State.ShouldBeOfType<Phase10GameState.Complete>();
        Complete.Winner.PlayerName.ShouldBe("Bob"); // lower score wins
    }

    [Fact]
    public async Task ActiveStandings_OrdersPlayersByHighestPhaseThenLowestScore()
    {
        await StartGameWithPlayers("Alice", "Bob", "Charlie");
        InProgress.Game.Players[0] = new Phase10Player.Active("Alice",   3, 10);
        InProgress.Game.Players[1] = new Phase10Player.Active("Bob",     3,  5);
        InProgress.Game.Players[2] = new Phase10Player.Active("Charlie", 1,  0);

        var standings = InProgress.ActiveStandings.ToList();

        standings[0].Name.ShouldBe("Bob");     // phase 3, score 5
        standings[1].Name.ShouldBe("Alice");   // phase 3, score 10
        standings[2].Name.ShouldBe("Charlie"); // phase 1
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task StartGameWithPlayers(params string[] names)
    {
        Setup.PlayerNames.Clear();
        Setup.PlayerNames.AddRange(names);
        await _sut.StartGameAsync();
        // Reset save call count so tests can assert on calls made after setup
        _store.ClearReceivedCalls();
    }
}
