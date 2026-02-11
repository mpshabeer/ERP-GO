using Refit;
using ERPGOAPPLICATION.Interfaces;
using ERPGOINFRASTRUCTURE; // For AddInfrastructureServices
using ERPGoEdition.Shared;   // For AddApiClientServices
using ERPGoEdition.Shared.Services;
using ERPGoEdition.Services;
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

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();

            // Auth Configuration
            // Read from AppPreferences, default to Direct Mode (false)
            bool useApi = Preferences.Get("UseApi", false); 

            if (useApi)
            {
                // API Mode - Use localhost for testing, change IP for device
                builder.Services.AddApiClientServices(new Uri("https://localhost:7123"));
            }
            else
            {
                // Direct Mode
                string connectionString = "Server=DESKTOP-U0M528H\\SQLEXPRESS;Database=ERPGoEdition;User Id=sa;Password=8190;TrustServerCertificate=True;MultipleActiveResultSets=true";
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
