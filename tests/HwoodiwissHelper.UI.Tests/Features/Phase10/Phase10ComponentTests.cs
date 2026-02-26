using Bunit;
using HwoodiwissHelper.UI.Pages.Games.Phase10;
using HwoodiwissHelper.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace HwoodiwissHelper.UI.Tests.Features.Phase10;

public class Phase10ComponentTests : BunitContext
{
    private readonly IPhase10GameService _gameService = Substitute.For<IPhase10GameService>();

    public Phase10ComponentTests()
    {
        Services.AddSingleton(_gameService);
        Services.AddSingleton(Substitute.For<IAppStateStore>());
    }

    // ── Setup view ────────────────────────────────────────────────────────────

    [Fact]
    public void SetupView_IsRendered_WhenStateIsSetup()
    {
        _gameService.State.Returns(new Phase10GameState.Setup(["", ""]));

        var cut = Render<Pages.Games.Phase10.Phase10>();

        cut.Find("h2").TextContent.ShouldBe("New Game");
    }

    [Fact]
    public void SetupView_StartButton_IsDisabled_WhenNamesAreInvalid()
    {
        _gameService.State.Returns(new Phase10GameState.Setup(["", ""]));

        var cut = Render<Pages.Games.Phase10.Phase10>();

        var startButton = cut.FindAll("button").First(b => b.TextContent.Trim() == "Start Game");
        startButton.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void SetupView_StartButton_IsEnabled_WhenNamesAreValid()
    {
        _gameService.State.Returns(new Phase10GameState.Setup(["Alice", "Bob"]));

        var cut = Render<Pages.Games.Phase10.Phase10>();

        var startButton = cut.FindAll("button").First(b => b.TextContent.Trim() == "Start Game");
        startButton.HasAttribute("disabled").ShouldBeFalse();
    }

    [Fact]
    public void SetupView_AddPlayerButton_IsDisabled_WhenAtMaximum()
    {
        _gameService.State.Returns(new Phase10GameState.Setup(["A", "B", "C", "D", "E", "F"]));

        var cut = Render<Pages.Games.Phase10.Phase10>();

        var addButton = cut.FindAll("button").First(b => b.TextContent.Contains("Add Player"));
        addButton.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void SetupView_RemoveButton_IsDisabled_WhenAtMinimum()
    {
        _gameService.State.Returns(new Phase10GameState.Setup(["", ""]));

        var cut = Render<Pages.Games.Phase10.Phase10>();

        var removeButtons = cut.FindAll("button").Where(b => b.TextContent.Trim() == "Remove").ToList();
        removeButtons.ShouldAllBe(b => b.HasAttribute("disabled"));
    }

    // ── Active game view ──────────────────────────────────────────────────────

    [Fact]
    public void ActiveGameView_IsRendered_WhenGameIsInProgress()
    {
        var game = new Phase10Game
        {
            CurrentRound = 2,
            Players =
            [
                new Phase10Player.Active("Alice", 1, 0),
                new Phase10Player.Active("Bob",   1, 0),
            ],
        };
        _gameService.State.Returns(new Phase10GameState.InProgress(
            game,
            [new RoundEntryModel(), new RoundEntryModel()]));

        var cut = Render<Pages.Games.Phase10.Phase10>();

        cut.Find("h2").TextContent.ShouldBe("Round 2");
    }

    [Fact]
    public void ActiveGameView_ShowsPlayerCardsForEachPlayer()
    {
        var game = new Phase10Game
        {
            Players =
            [
                new Phase10Player.Active("Alice", 1, 0),
                new Phase10Player.Active("Bob",   1, 0),
            ],
        };
        _gameService.State.Returns(new Phase10GameState.InProgress(
            game,
            [new RoundEntryModel(), new RoundEntryModel()]));

        var cut = Render<Pages.Games.Phase10.Phase10>();

        var cardTitles = cut.FindAll(".card-title").Select(e => e.TextContent).ToList();
        cardTitles.ShouldContain("Alice");
        cardTitles.ShouldContain("Bob");
    }

    // ── Game-complete view ────────────────────────────────────────────────────

    [Fact]
    public void GameCompleteView_IsRendered_WhenGameIsComplete()
    {
        var winner = new Phase10Player.Completed("Alice", 20);
        var game = new Phase10Game
        {
            Players = [winner, new Phase10Player.Active("Bob", 3, 10)],
        };
        _gameService.State.Returns(new Phase10GameState.Complete(game, winner, CompletedPlayerCount: 1));

        var cut = Render<Pages.Games.Phase10.Phase10>();

        cut.Find("h2").TextContent.ShouldBe("Final Standings");
        cut.Find(".alert-success strong").TextContent.ShouldBe("Alice wins!");
    }

    [Fact]
    public void GameCompleteView_ShowsTieMessage_WhenMultiplePlayersCompleted()
    {
        var p1 = new Phase10Player.Completed("Alice", 20);
        var p2 = new Phase10Player.Completed("Bob",   30);
        var game = new Phase10Game { Players = [p1, p2] };
        _gameService.State.Returns(new Phase10GameState.Complete(game, p1, CompletedPlayerCount: 2));

        var cut = Render<Pages.Games.Phase10.Phase10>();

        cut.Find(".alert-success").TextContent.ShouldContain("winner has lowest score");
    }
}
