using ERPGOAPPLICATION.Interfaces;
using ERPGoEdition.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ERPGoEdition.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddApiClientServices(this IServiceCollection services, Uri baseAddress)
    {
        // Auth Services
        services.AddRefitClient<IAuthApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddScoped<IAuthService, AuthApiService>();

        // Unit Services
        services.AddRefitClient<IUnitApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddScoped<IUnitService, UnitApiService>();

        // Item Services
        services.AddRefitClient<IItemApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddScoped<IItemService, ItemApiService>();
        
        return services;
    }
}
