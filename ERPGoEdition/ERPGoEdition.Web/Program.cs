using ERPGoEdition.Shared; // For AddApiClientServices
using ERPGoEdition.Shared.Services;
using ERPGOINFRASTRUCTURE; // For AddInfrastructureServices
using ERPGoEdition.Web.Components;
using ERPGoEdition.Web.Services;
using MudBlazor.Services;
using ERPGOINFRASTRUCTURE.Services;
using ERPGOAPPLICATION.Interfaces;
using QuestPDF.Infrastructure;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the ERPGoEdition.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddSingleton<ITabService, TabService>();
builder.Services.AddMudServices();
builder.Services.AddScoped<IAppSettingsService, AppSettingsService>();
builder.Services.AddSingleton<ICategoryService, CategoryService>(); // Singleton for Blazor Server
builder.Services.AddBlazoredLocalStorage();

// Auth for interactive Server components
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
    })
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";
    });

// Auth Services (Web uses Direct DB connection to avoid starting separate API)
// builder.Services.AddApiClientServices(new Uri("https://localhost:7123")); 
string connectionString = "Server=.\\SQLEXPRESS01;Database=ERPGoEdition;User Id=sa1;Password=5018;TrustServerCertificate=True;MultipleActiveResultSets=true";
builder.Services.AddInfrastructureServices(connectionString);

// PDF generation — directly in-process using ISalesService (no separate API needed)
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
builder.Services.AddScoped<IInvoicePdfService, InvoicePdfService>();
builder.Services.AddScoped<IPurchasePdfService, PurchasePdfService>();
builder.Services.AddScoped<IStockReportPdfService, StockReportPdfService>();
builder.Services.AddScoped<IReportPdfService, ReportPdfService>();
builder.Services.AddScoped<IPurchasePdfService, PurchasePdfService>();



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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(ERPGoEdition.Shared._Imports).Assembly);

// Seed Data
await ERPGOINFRASTRUCTURE.Persistence.AccountingDbSeeder.SeedAsync(app.Services);

app.Run();
