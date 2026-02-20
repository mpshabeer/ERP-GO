using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class StockAdjustmentService : IStockAdjustmentService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public StockAdjustmentService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<string> CreateAdjustment(StockAdjustmentHeader header)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 1. Generate Adjustment No (Simple Auto-Increment Logic or random for now)
                // In production, better to use sequence or separate table.
                // Let's use SA-yyyyMMdd-HHmmss
                header.AdjustmentNo = $"SA-{DateTime.Now:yyyyMMdd-HHmmss}";
                header.Date = DateTime.Now;

                // 3. Process Details (Prepare for Insert)
                // We must nullify navigation properties to prevent EF from trying to insert defined entities (Item, ItemUnit, Unit)
                // We only want to save the FKs (ItemId, ItemUnitId)
                foreach (var detail in header.Details)
                {
                    detail.Item = null;
                    detail.ItemUnit = null;
                }

                // 2. Add Header
                context.StockAdjustmentHeaders.Add(header);
                await context.SaveChangesAsync(); // Get HeaderId

                // 3. Process Details
                foreach (var detail in header.Details)
                {
                    detail.HeaderId = header.Id;
                    
                    // Fetch Multiplier if Unit is selected
                    decimal multiplier = 1;
                    if (detail.ItemUnitId.HasValue)
                    {
                        var itemUnit = await context.ItemUnits.FindAsync(detail.ItemUnitId);
                        if (itemUnit != null)
                        {
                            multiplier = itemUnit.QtyPerBaseUnit;
                        }
                    }

                    // A. Calculate Adjusted Qty (Already comes as NEW - SYSTEM in UNIT from UI)
                    // We trust the AdjustedQty from UI is in the SELECTED UNIT.
                    
                    // B. Calculate Base Qty Impact for Stock Update
                    var baseAdjustedQty = detail.AdjustedQty * multiplier;

                    // context.StockAdjustmentDetails.Add(detail); // Redundant. Already saved via Header.


                    // C. Update Item Current Stock (Always in Base Unit)
                    var item = await context.Items.FindAsync(detail.ItemId);
                    if (item != null)
                    {
                        item.CurrentStock += baseAdjustedQty;
                        context.Items.Update(item);
                    }

                    // D. Add Stock Ledger Entry
                    var ledger = new StockLedger
                    {
                        Date = header.Date,
                        ItemId = detail.ItemId,
                        ItemUnitId = detail.ItemUnitId,
                        // Ledger Qty: Typically store what was physically counted/adjusted in that unit.
                        // Or converts to base? Usually ledger tracks the specific unit transaction.
                        Qty = detail.AdjustedQty, 
                        Rate = 0, // Rate is usually 0 or Weighted Average Cost if we track it. For now 0.
                        TransactionType = "ADJUSTMENT",
                        RefId = header.AdjustmentNo,
                        Notes = $"Stock Adjustment: {header.Notes} | {detail.Notes}"
                    };
                    context.StockLedger.Add(ledger);
                }


                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        return header.AdjustmentNo;
    }

    public async Task<List<StockAdjustmentHeader>> GetAdjustments()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StockAdjustmentHeaders
            .OrderByDescending(x => x.Date)
            .ToListAsync();
    }

    public async Task<StockAdjustmentHeader?> GetAdjustment(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StockAdjustmentHeaders
            .Include(x => x.Details)
            .ThenInclude(d => d.Item)
            .Include(x => x.Details)
            .ThenInclude(d => d.ItemUnit)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
