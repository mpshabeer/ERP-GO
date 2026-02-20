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

        // Customer Services
        services.AddRefitClient<ICustomerApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddScoped<ICustomerService, CustomerApiService>();

        // Supplier Services
        services.AddRefitClient<ISupplierApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddScoped<ISupplierService, SupplierApiService>();

        // Stock Services
        services.AddRefitClient<IStockApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddScoped<IStockService, StockApiService>();

        // Sales Services
        services.AddRefitClient<ISalesApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddScoped<ISalesService, SalesApiService>();
        
        // Purchase Services
        services.AddRefitClient<IPurchaseApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddScoped<IPurchaseService, PurchaseApiService>();

        // Category Services
        services.AddRefitClient<ICategoryApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = baseAddress);
        services.AddScoped<ICategoryService, CategoryApiService>();

        return services;
    }
}
