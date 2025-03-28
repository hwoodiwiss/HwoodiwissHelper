namespace HwoodiwissHelper.Core;

public sealed record Unit
{
    public static Unit Instance { get;  } = new();

    private Unit()
    {
        
    }
}
