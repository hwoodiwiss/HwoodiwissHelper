namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

public sealed class ForkViewModel
{
    public required bool Selected { get; set; }

    public required long ForkId { get; set; }

    public required string FullName { get; set; }
    
    public required string Name { get; set; }
    
    public required string Owner { get; set; }
    
    public bool IsDeleting { get; set; }

    public bool IsDeleted
    {
        get;
        set
        {
            IsDeleting = false;
            field = value;
        }
    }
}
