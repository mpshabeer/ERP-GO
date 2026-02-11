using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class SupplierService : ISupplierService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public SupplierService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Suppliers.Where(s => s.IsActive).ToListAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Supplier> AddSupplierAsync(Supplier supplier)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();
        return supplier;
    }

    public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var existingSupplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Id == supplier.Id);
        if (existingSupplier != null)
        {
            context.Entry(existingSupplier).CurrentValues.SetValues(supplier);
            await context.SaveChangesAsync();
            return existingSupplier;
        }
        return supplier;
    }

    public async Task DeleteSupplierAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var supplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        if (supplier != null)
        {
            supplier.IsActive = false;
            await context.SaveChangesAsync();
        }
    }
}
