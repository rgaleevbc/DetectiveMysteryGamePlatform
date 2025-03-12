using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DetectiveMysteryGamePlatform.Client;
using DetectiveMysteryGamePlatform.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient
builder.Services.AddScoped(sp => 
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    return httpClient;
});

// Add LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Add Services
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<GameHubService>();

await builder.Build().RunAsync();
