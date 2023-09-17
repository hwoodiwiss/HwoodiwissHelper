using HwoodiwissHelper.Infrastructure.Filters;

namespace HwoodiwissHelper.Features.Github;

public static partial class GithubWebhookEndpoints
{
    public static IEndpointRouteBuilder MapGithubEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/github");

        group.MapPost("/webhook", () => { })
            .AddEndpointFilterFactory(GithubSecretValidatorFilter.Factory);
        
        return builder;
    }
}
