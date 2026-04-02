namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

internal interface IPhase10GameService
{
    Phase10GameState State { get; }

    Task LoadGameAsync();
    void AddPlayer();
    void RemovePlayer(int index);
    Task StartGameAsync();
    Task SubmitRoundAsync();
    Task EditRoundAsync(int roundIndex, RoundEntryModel[] updatedEntries);
    Task AbandonGameAsync();
    Task NewGameAsync();
}
