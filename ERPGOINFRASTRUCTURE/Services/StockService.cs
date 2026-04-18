using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;
using ERPGODomain.DTOs;

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
        using var context = await _contextFactory.CreateDbContextAsync();
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        { 
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
        });
    }

    public async Task<decimal> GetStock(int itemId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var item = await context.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == itemId);
        return item?.CurrentStock ?? 0;
    }

    public async Task AddStockTransactions(List<StockLedger> entries)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                foreach (var entry in entries)
                {
                    context.StockLedger.Add(entry);
                    var item = await context.Items.FindAsync(entry.ItemId);
                    if (item != null)
                    {
                        item.CurrentStock += entry.Qty;
                        context.Items.Update(item);
                    }
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
    }
    public async Task SaveOpeningStock(ItemOpeningStock entry)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // 1. Check for existing Opening Stock for this Item
                var existingOpening = await context.ItemOpeningStocks
                    .FirstOrDefaultAsync(x => x.ItemId == entry.ItemId);

                if (existingOpening != null)
                {
                    // A. Reverse effect on Current Stock
                    var item = await context.Items.FindAsync(entry.ItemId);
                    if (item != null)
                    {
                        item.CurrentStock -= existingOpening.Qty;
                        context.Items.Update(item);
                    }

                    // B. Remove old StockLedger entry (Type = OPENING)
                    var oldLedgerEntries = await context.StockLedger
                        .Where(x => x.ItemId == entry.ItemId && x.TransactionType == "OPENING")
                        .ToListAsync();
                    
                    context.StockLedger.RemoveRange(oldLedgerEntries);

                    // C. Log History (Old Entry Replaced)
                    context.OpeningStockHistories.Add(new OpeningStockHistory
                    {
                        ItemId = existingOpening.ItemId,
                        Qty = existingOpening.Qty,
                        Rate = existingOpening.Rate,
                        ItemUnitId = existingOpening.ItemUnitId,
                        Date = DateTime.Now,
                        Action = "Replaced",
                        Notes = $"Replaced by new entry on {DateTime.Now:g} | Old Notes: {existingOpening.Notes}"
                    });

                    // D. Remove old ItemOpeningStock
                    context.ItemOpeningStocks.Remove(existingOpening);
                }

                // 2. Add New Opening Stock
                context.ItemOpeningStocks.Add(entry);

                // 3. Log History (New Entry Created)
                context.OpeningStockHistories.Add(new OpeningStockHistory
                {
                    ItemId = entry.ItemId,
                    Qty = entry.Qty,
                    Rate = entry.Rate,
                    ItemUnitId = entry.ItemUnitId,
                    Date = DateTime.Now,
                    Action = "Created",
                    Notes = entry.Notes
                });

                // 4. Update StockLedger
                context.StockLedger.Add(new StockLedger
                {
                    Date = entry.Date,
                    ItemId = entry.ItemId,
                    Qty = entry.Qty,
                    Rate = entry.Rate,
                    ItemUnitId = entry.ItemUnitId,
                    TransactionType = "OPENING",
                    RefId = "OS-" + entry.Id, // Note: Id is 0 here.
                    Notes = entry.Notes
                });

                // 5. Update Item Current Stock
                var itemToUpdate = await context.Items.FindAsync(entry.ItemId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.CurrentStock += entry.Qty;
                    context.Items.Update(itemToUpdate);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
    

    public async Task<List<ItemOpeningStock>> GetItemOpeningStocks()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ItemOpeningStocks
            .Include(x => x.Item)
            .Include(x => x.ItemUnit)
            .OrderByDescending(x => x.Date)
            .ToListAsync();
    }

    public async Task<List<OpeningStockHistory>> GetOpeningStockHistories()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        // Return recent 500 history items
        return await context.OpeningStockHistories
            .OrderByDescending(x => x.Date)
            .Take(500)
            .ToListAsync();
    }

    public async Task<ItemOpeningStock?> GetItemOpeningStock(int itemId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.ItemOpeningStocks
            .Include(x => x.Item)
            .Include(x => x.ItemUnit)
            .FirstOrDefaultAsync(x => x.ItemId == itemId);
    }

    public async Task<List<OpeningStockHistory>> GetOpeningStockHistory(int itemId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.OpeningStockHistories
            .Where(x => x.ItemId == itemId)
            .OrderByDescending(x => x.Date)
            .ToListAsync();
    }

    public async Task<PaginatedResult<OpeningStockHistory>> GetPaginatedOpeningStockHistory(string? search, int page, int pageSize)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.OpeningStockHistories
             .Include(x => x.Item) // Include Item for Search
             .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
             query = query.Where(x => x.Notes.Contains(search) 
                                   || x.Action.Contains(search)
                                   || x.Item.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.Date)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Since we Included Item, the Items list will have Item populated.
        // EF Core might fixup references automatically if tracked, but we are using new context per request usually.
        // Include ensures it's loaded.
        
        return new PaginatedResult<OpeningStockHistory>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task<List<StockReportLine>> GetStockReportAsync(StockReportRequest request)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        var query = context.Items
            .Include(i => i.Category)
            .Include(i => i.BaseUnit)
            .AsNoTracking();

        if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
        {
            query = query.Where(i => i.CategoryId == request.CategoryId.Value);
        }

        if (request.ItemId.HasValue && request.ItemId.Value > 0)
        {
            query = query.Where(i => i.Id == request.ItemId.Value);
        }

        var items = await query.ToListAsync();

        if (!items.Any()) return new List<StockReportLine>();

        var itemIds = items.Select(i => i.Id).ToList();
        
        // We load all ledger entries for the items to calculate opening and periodic movement 
        // (doing this in memory avoids very complex SQL aggregation overhead when dataset is small/medium).
        var ledgers = await context.StockLedger
            .AsNoTracking()
            .Where(l => itemIds.Contains(l.ItemId))
            .ToListAsync();

        var reportLines = new List<StockReportLine>();

        foreach (var item in items)
        {
            var itemLedgers = ledgers.Where(l => l.ItemId == item.Id).ToList();

            var openingQty = itemLedgers
                .Where(l => l.Date.Date < request.FromDate.Date)
                .Sum(l => l.Qty);

            var inwardQty = itemLedgers
                .Where(l => l.Date.Date >= request.FromDate.Date && l.Date.Date <= request.ToDate.Date && l.Qty > 0)
                .Sum(l => l.Qty);

            var outwardQty = itemLedgers
                .Where(l => l.Date.Date >= request.FromDate.Date && l.Date.Date <= request.ToDate.Date && l.Qty < 0)
                .Sum(l => Math.Abs(l.Qty));

            reportLines.Add(new StockReportLine
            {
                ItemId = item.Id,
                ItemCode = item.ItemCode ?? "",
                ItemName = item.Name ?? "",
                CategoryName = item.Category?.Name ?? "Uncategorized",
                UnitName = item.BaseUnit?.Name ?? "Unit",
                OpeningQty = openingQty,
                InwardQty = inwardQty,
                OutwardQty = outwardQty,
                ClosingQty = openingQty + inwardQty - outwardQty,
                ItemRate = item.PurchaseRate,
                StockValue = (openingQty + inwardQty - outwardQty) * item.PurchaseRate
            });
        }

        return reportLines.OrderBy(r => r.ItemName).ToList();
    }
}
