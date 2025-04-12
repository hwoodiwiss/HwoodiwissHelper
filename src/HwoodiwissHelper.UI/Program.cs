using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HwoodiwissHelper.UI;
using HwoodiwissHelper.UI.Extensions;
using HwoodiwissHelper.UI.Features.GitHubForkCleaner.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(builder.HostEnvironment);
builder.Services.ConfigureServices();
builder.Services.ConfigureGitHubServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
