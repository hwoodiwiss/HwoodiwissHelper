using Hwoodiwiss.Extensions.Hosting;
using HwoodiwissHelper.Extensions;

var app = HwoodiwissApplication.CreateBuilder(args)
    .ConfigureApplication()
    .ConfigureOptions(opt => opt.HostStaticAssets = true)
    .Build();

await app
    .ConfigureRequestPipeline()
    .RunAsync();
