using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TimeLens.BlazorUI;
using TimeLens.BlazorUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient trỏ tới WebAPI
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5227/")
});

// MudBlazor
builder.Services.AddMudServices();

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JournalService>();
builder.Services.AddScoped<ConversationService>();

await builder.Build().RunAsync();