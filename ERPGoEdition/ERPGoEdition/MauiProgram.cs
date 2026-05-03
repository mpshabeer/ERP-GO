using Refit;
using ERPGOAPPLICATION.Interfaces;
using ERPGOINFRASTRUCTURE; // For AddInfrastructureServices
using ERPGoEdition.Shared;   // For AddApiClientServices
using ERPGoEdition.Shared.Services;
using ERPGoEdition.Services;
using ERPGOINFRASTRUCTURE.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using QuestPDF.Infrastructure;

namespace ERPGoEdition
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()


                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the ERPGoEdition.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<ITabService, TabService>();
            builder.Services.AddScoped<IAppSettingsService, AppSettingsService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

#if WINDOWS || MACCATALYST
            QuestPDF.Settings.License = LicenseType.Community;
            builder.Services.AddScoped<IInvoicePdfService, InvoicePdfService>();
            builder.Services.AddScoped<IPurchasePdfService, PurchasePdfService>();
            builder.Services.AddScoped<IStockReportPdfService, StockReportPdfService>();
            builder.Services.AddScoped<IReportPdfService, ReportPdfService>();
#else
            // QuestPDF native generation is not supported on Android/iOS.
            
            string pdfApiUrl = "https://localhost:7205";
#if ANDROID
            pdfApiUrl = "http://10.0.2.2:5209"; // Emulator loopback for local API
#elif IOS
            pdfApiUrl = "http://localhost:5209"; // iOS simulator
#endif
            
            builder.Services.AddHttpClient<ERPGoEdition.Services.RemotePdfService>(c =>
            {
                c.BaseAddress = new Uri(pdfApiUrl);
            });
            builder.Services.AddScoped<IInvoicePdfService>(sp => sp.GetRequiredService<ERPGoEdition.Services.RemotePdfService>());
            builder.Services.AddScoped<IPurchasePdfService>(sp => sp.GetRequiredService<ERPGoEdition.Services.RemotePdfService>());
            builder.Services.AddScoped<IStockReportPdfService>(sp => sp.GetRequiredService<ERPGoEdition.Services.RemotePdfService>());
            builder.Services.AddScoped<IReportPdfService>(sp => sp.GetRequiredService<ERPGoEdition.Services.RemotePdfService>());
#endif

            // Auth & API Configuration
#if WINDOWS || MACCATALYST
            bool useApi = false; // Desktop typically uses Direct DB Connection
#else
            bool useApi = true;  // Mobile MUST use the API backend
#endif

            if (useApi)
            {
                string apiBaseUrl = "https://localhost:7205";
#if ANDROID
                apiBaseUrl = "http://10.0.2.2:5209"; // Android emulator local API bypass
#elif IOS
                apiBaseUrl = "http://localhost:5209"; 
#endif
                builder.Services.AddApiClientServices(new Uri(apiBaseUrl));
            }
            else
            {
                // Direct Mode
                string connectionString = "Server=.\\SQLEXPRESS01;Database=ERPGoEdition;User Id=sa1;Password=5018;TrustServerCertificate=True;MultipleActiveResultSets=true";
                builder.Services.AddInfrastructureServices(connectionString);
            }



#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
