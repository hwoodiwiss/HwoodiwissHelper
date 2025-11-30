using Hwoodiwiss.Extensions.Hosting;
using HwoodiwissHelper.Extensions;

var app = HwoodiwissApplication.CreateBuilder(args)
    .ConfigureApplication()
    .Build();

await app
    .ConfigureRequestPipeline()
    .RunAsync();
