using Refit;
using ERPGOAPPLICATION.Interfaces;
using ERPGOINFRASTRUCTURE; // For AddInfrastructureServices
using ERPGoEdition.Shared;   // For AddApiClientServices
using ERPGoEdition.Shared.Services;
using ERPGoEdition.Services;
using ERPGOINFRASTRUCTURE.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;


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

            // Auth Configuration
            // FORCE DIRECT MODE for debugging/consistency
            bool useApi = false; // Preferences.Get("UseApi", true); 

            if (useApi)
            {
                // API Mode - Use localhost for testing, change IP for device
                builder.Services.AddApiClientServices(new Uri("https://localhost:7205"));
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
