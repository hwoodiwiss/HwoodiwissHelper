using HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.Extensions;

public static class RepositoryExtensions
{
    public static ForkViewModel ToForkViewModel(this Repository repository) =>
        new()
        {
            Selected = false,
            ForkId = repository.Id,
            FullName = repository.FullName,
            Name = repository.Name,
            Owner = repository.Owner.Login,
        };
}
