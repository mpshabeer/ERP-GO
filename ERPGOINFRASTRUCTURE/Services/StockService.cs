using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class StockService : IStockService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public StockService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task AddStockTransaction(StockLedger entry)
    {
        // Note: This method should ideally be participating in a larger transaction scope 
        // if called from SalesService. Since we are using IDbContextFactory, dealing with single transactions 
        // across services can be tricky without passing the DbContext reference.
        // For simplicity in this Blazor Server architecture, we'll assume the Caller (SalesService) 
        // handles the overarching unit of work IF we refactor to pass context, OR 
        // we accept that this method does its own small transaction.
        // 
        // *Correction*: To support the requirement "Start transaction so we can start with invoice...",
        // the best approach in EF Core with Factory is to let the SalesService handle the transaction 
        // and do the logic there, OR pass the context. 
        //
        // However, keeping services decoupled (interface-based) usually implies they manage their own specific data.
        // I will implement this as a standalone operation first. If SalesService needs to be atomic conformant,
        // we might move this logic into SalesService or share a context. 
        // given the prompt "Start transaction", I will design SalesService to be the coordinator.
        
        using var context = await _contextFactory.CreateDbContextAsync();
        using var transaction = await context.Database.BeginTransactionAsync();

        try 
        {
            context.StockLedger.Add(entry);
            
            var item = await context.Items.FindAsync(entry.ItemId);
            if (item != null)
            {
                item.CurrentStock += entry.Qty;
                context.Items.Update(item);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<decimal> GetStock(int itemId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var item = await context.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == itemId);
        return item?.CurrentStock ?? 0;
    }
}
