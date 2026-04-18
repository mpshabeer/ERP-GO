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
        return await context.Suppliers.ToListAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Supplier> AddSupplierAsync(Supplier supplier)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        if (supplier.AccountId == null || supplier.AccountId <= 0)
        {
            var creditorsHead = await context.AccountHeads.FirstOrDefaultAsync(h => h.Name == "Sundry Creditors");
            if (creditorsHead != null)
            {
                var account = new Account
                {
                    Name = supplier.Name + " Account",
                    Code = string.Empty,
                    AccountHeadId = creditorsHead.Id,
                    IsDefault = false,
                    IsActive = true
                };
                context.Accounts.Add(account);
                await context.SaveChangesAsync();
                
                supplier.AccountId = account.Id;
            }
        }

        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync();
        return supplier;
    }

    public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
    {
        if (supplier.Code == "S1000") throw new InvalidOperationException("Default System Supplier cannot be modified.");
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
            if (supplier.Code == "S1000") throw new InvalidOperationException("Default System Supplier cannot be deleted.");
            supplier.IsActive = false;
            await context.SaveChangesAsync();
        }
    }
}
