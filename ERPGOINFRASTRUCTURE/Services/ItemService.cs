
using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class ItemService : IItemService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ItemService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Item>> GetAllItemsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Items
            .Include(i => i.BaseUnit)
            .Include(i => i.Category)
            .Include(i => i.ItemUnits)
            .ThenInclude(iu => iu.Unit)
            //.Where(i => i.IsActive) // Allowing all items for UI filtering
            .ToListAsync();
    }

    public async Task<Item?> GetItemByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Items
            .Include(i => i.BaseUnit)
            .Include(i => i.ItemUnits)
            .ThenInclude(iu => iu.Unit)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Item> AddItemAsync(Item item)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // Prevent tracking issues with related entities by nullifying navigation properties.
            // EF Core will use the foreign keys (UnitId, CategoryId) instead.
            item.BaseUnit = null;
            item.Category = null;
            foreach (var unit in item.ItemUnits)
            {
                unit.Unit = null;
                unit.Item = null;
            }

            context.Items.Add(item);

            // Handle Opening Stock
            if (item.CurrentStock > 0)
            {
                var stockEntry = new StockLedger
                {
                    Date = DateTime.Now,
                    // ItemId is not yet generated, but EF Core will handle the FK fixup 
                    // IF we add it to the context and let SaveChanges handle it.
                    // However, relying on navigation property is safer if relations are set up.
                    // Since StockLedger is not a navigation property of Item (it's the other way round), 
                    // we depend on SaveChanges to generate Item.Id first. 
                    // But wait, if we add it to context, EF might not know the Id yet.
                    // Best to Save Item first, then Add Stock Ledger.

                    ItemId = item.Id, // This will be 0 here
                    Qty = item.CurrentStock,
                    TransactionType = "OPENING",
                    RefId = "OP-STOCK",
                    Notes = "Opening Stock"
                };

                // We need to save item first to get Id, OR use navigation property if we added one to Item (we didn't).
                await context.SaveChangesAsync();

                // Now Item.Id is populated
                stockEntry.ItemId = item.Id;
                context.StockLedger.Add(stockEntry);
                await context.SaveChangesAsync();
            }
            else
            {
                await context.SaveChangesAsync();
            }

            return item;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public async Task<Item> UpdateItemAsync(Item item)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // Load existing item including children to manage the collection
        var existingItem = await context.Items
            .Include(i => i.ItemUnits)
            .ThenInclude(iu => iu.Unit)
            .FirstOrDefaultAsync(i => i.Id == item.Id);

        if (existingItem != null)
        {
            // Update scalar properties
            context.Entry(existingItem).CurrentValues.SetValues(item);

            // Prevent tracking issues on update by nullifying the navigation properties
            item.BaseUnit = null;
            item.Category = null;
            existingItem.BaseUnit = null;
            existingItem.Category = null;

            // Handle ItemUnits Collection (Add/Update/Delete)
            // 1. Delete removed units
            foreach (var existingUnit in existingItem.ItemUnits.ToList())
            {
                if (!item.ItemUnits.Any(u => u.Id == existingUnit.Id && u.Id != 0))
                {
                    bool isReferenced = await context.StockLedger.AnyAsync(s => s.ItemUnitId == existingUnit.Id)
                                     || await context.SalesInvoiceItems.AnyAsync(s => s.ItemUnitId == existingUnit.Id)
                                     || await context.PurchaseItems.AnyAsync(p => p.ItemUnitId == existingUnit.Id);

                    if (isReferenced)
                    {
                        throw new Exception($"Cannot remove unit '{existingUnit.Unit?.Name ?? "ID " + existingUnit.Id}' as it is currently used in transactions.");
                    }
                    context.ItemUnits.Remove(existingUnit);
                }
            }

            // 2. Add or Update units
            foreach (var unit in item.ItemUnits)
            {
                var existingUnit = existingItem.ItemUnits.FirstOrDefault(u => u.Id == unit.Id && u.Id != 0);

                if (existingUnit != null)
                {
                    // Update
                    context.Entry(existingUnit).CurrentValues.SetValues(unit);
                }
                else
                {
                    // Add
                    unit.Unit = null;
                    unit.Item = null;
                    existingItem.ItemUnits.Add(unit);
                }
            }

            await context.SaveChangesAsync();
            return existingItem;
        }

        return item;
    }

    public async Task DeleteItemAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var item = await context.Items.FindAsync(id);
        if (item != null)
        {
            item.IsActive = false;
            context.Items.Update(item);
            await context.SaveChangesAsync();
        }
    }
    public async Task<string> GetNextItemCodeAsync(string prefix, int startNumber)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        // Fetch valid codes that start with the prefix
        // We fetch potentially relevant codes. String comparison in SQL can be tricky for "Max" number.
        // So we grab the latest ones.
        var codes = await context.Items
            .Where(i => i.ItemCode != null && i.ItemCode.StartsWith(prefix))
            .Select(i => i.ItemCode)
            .ToListAsync();

        int maxNumber = 0;

        foreach (var code in codes)
        {
            if (string.IsNullOrEmpty(code)) continue;
            
            // Remove prefix
            string numberPart = code.Substring(prefix.Length);
            
            // Try parse
            if (int.TryParse(numberPart, out int number))
            {
                if (number > maxNumber) maxNumber = number;
            }
        }

        int nextNumber = maxNumber + 1;
        if (nextNumber < startNumber) nextNumber = startNumber;

        return $"{prefix}{nextNumber}";
    }
}
