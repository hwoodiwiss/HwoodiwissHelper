using Hwoodiwiss.Extensions.Hosting;
using HwoodiwissHelper.Endpoints;
using HwoodiwissHelper.Features.Benchmarks.Endpoints;
using HwoodiwissHelper.Features.GitHub.Endpoints;
using HwoodiwissHelper.Features.GitHub.Extension;

var builder = HwoodiwissApplication.CreateBuilder(args)
    .ConfigureOptions(opt => opt.HostStaticAssets = true);

builder.Services.ConfigureGitHubServices(builder.Configuration, builder.Environment);

var app = builder.Build();

if (app.Environment.IsEnvironment("Benchmarks"))
{
    app.MapBenchmarkEndpoints();
}

await app
    .MapSurpriseEndpoints()
    .MapGitHubEndpoints()
    .RunAsync();
