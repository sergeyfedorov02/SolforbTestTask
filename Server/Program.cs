using Microsoft.EntityFrameworkCore;
using Radzen;
using SolvoTestTask.Server.Components;
using SolvoTestTask.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
      .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();
builder.Services.AddRadzenComponents();

builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "SolvoTestTaskTheme";
    options.Duration = TimeSpan.FromDays(365);
});
builder.Services.AddHttpClient();

// настраиваем провайдера дл€ Ѕƒ (PostgreSQL)
builder.Services.AddDbContext<SolvoDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("SolvoConnection"));
});

builder.Services.AddTransient<Func<SolvoDBContext>>(provider =>
   () => provider.CreateScope().ServiceProvider.GetRequiredService<SolvoDBContext>());

var app = builder.Build();


var forwardingOptions = new ForwardedHeadersOptions()
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
};
forwardingOptions.KnownNetworks.Clear();
forwardingOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardingOptions);
    

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies(typeof(SolvoTestTask.Client._Imports).Assembly);

// убеждаемс€, что база создана (если нет -> создаем)
var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<SolvoDBContext>();

db.Database.SetCommandTimeout(60);
db.Database.EnsureCreated();

app.Run();