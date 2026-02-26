namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

internal interface IPhase10GameService
{
    Phase10GameState State { get; }

    Task LoadGameAsync();
    void AddPlayer();
    void RemovePlayer(int index);
    Task StartGameAsync();
    Task SubmitRoundAsync();
    Task AbandonGameAsync();
    Task NewGameAsync();
}
