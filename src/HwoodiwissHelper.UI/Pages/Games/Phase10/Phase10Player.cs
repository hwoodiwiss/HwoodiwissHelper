using Dunet;

namespace HwoodiwissHelper.UI.Pages.Games.Phase10;

[Union]
internal partial record Phase10Player
{
    public partial record Active(string PlayerName, int Phase, int Score);
    public partial record Completed(string PlayerName, int Score);
}

