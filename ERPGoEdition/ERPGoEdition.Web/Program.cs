using ERPGoEdition.Shared; // For AddApiClientServices
using ERPGoEdition.Shared.Services;
using ERPGOINFRASTRUCTURE; // For AddInfrastructureServices
using ERPGoEdition.Web.Components;
using ERPGoEdition.Web.Services;
using MudBlazor.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the ERPGoEdition.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddSingleton<ITabService, TabService>();
builder.Services.AddMudServices();

// Auth Services (Web uses Direct DB connection to avoid starting separate API)
// builder.Services.AddApiClientServices(new Uri("https://localhost:7123")); 
string connectionString = "Server=DESKTOP-U0M528H\\SQLEXPRESS;Database=ERPGODB;User Id=sa;Password=8190;TrustServerCertificate=True;MultipleActiveResultSets=true";
builder.Services.AddInfrastructureServices(connectionString);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(ERPGoEdition.Shared._Imports).Assembly);

app.Run();
