namespace HwoodiwissHelper.Endpoints;

public static class SurpriseEndpoints
{
    public static IEndpointRouteBuilder MapSurpriseEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/surprise");

        group.MapGet("/next", (IConfiguration configuration) =>
            {
                var surprises = configuration.GetSection("Surprises").Get<string[]>() ?? [];

                return surprises.Length switch
                {
                    0 => Results.Redirect("/"),
                    _ => Results.Redirect(surprises[Random.Shared.Next(0, surprises.Length - 1)], true)
                };
            })
            .WithDescription("Gets the next surprise.")
            .Produces(302);

        return builder;
    }
}
