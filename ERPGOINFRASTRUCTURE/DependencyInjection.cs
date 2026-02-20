using ERPGOAPPLICATION.Interfaces;
using ERPGOINFRASTRUCTURE.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERPGOINFRASTRUCTURE;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<Persistence.ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, 
                builder => builder.EnableRetryOnFailure()));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<ISalesService, SalesService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();
        
        return services;
    }
}
