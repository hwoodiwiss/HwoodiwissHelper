using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HwoodiwissHelper.UI;
using HwoodiwissHelper.UI.Pages.Games.Phase10;
using HwoodiwissHelper.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAppStateStore, LocalStorageAppStateStore>();
builder.Services.AddScoped<IPhase10GameService, Phase10GameService>();

await builder.Build().RunAsync();
