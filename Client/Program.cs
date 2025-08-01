using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using SolforbTestTask.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddRadzenComponents();

builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "SolforbTestTaskTheme";
    options.Duration = TimeSpan.FromDays(365);
});
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient();

builder.Services.AddTransient<IBalanceService, BalanceService>();

var host = builder.Build();
await host.RunAsync();