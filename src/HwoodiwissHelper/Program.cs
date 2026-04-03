using Hwoodiwiss.Extensions.Hosting;
using HwoodiwissHelper.Endpoints;
using HwoodiwissHelper.Features.GitHub.Endpoints;
using HwoodiwissHelper.Features.GitHub.Extension;

var builder = HwoodiwissApplication.CreateBuilder(args)
    .ConfigureOptions(opt => opt.HostStaticAssets = true);

builder.Services.ConfigureGitHubServices(builder.Configuration);

var app = builder.Build();

await app
    .MapSurpriseEndpoints()
    .MapGitHubEndpoints()
    .RunAsync();
