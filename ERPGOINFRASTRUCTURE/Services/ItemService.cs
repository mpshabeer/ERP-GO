
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
            .Include(i => i.ItemUnits)
            .ThenInclude(iu => iu.Unit)
            .Where(i => i.IsActive)
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
        using var context = await _contextFactory.CreateDbContextAsync();
        
        // Prevent tracking issues with related entities
        if (item.BaseUnit != null) context.Entry(item.BaseUnit).State = EntityState.Unchanged;
        foreach (var unit in item.ItemUnits)
        {
            if (unit.Unit != null) context.Entry(unit.Unit).State = EntityState.Unchanged;
        }

        context.Items.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<Item> UpdateItemAsync(Item item)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // Load existing item including children to manage the collection
        var existingItem = await context.Items
            .Include(i => i.ItemUnits)
            .FirstOrDefaultAsync(i => i.Id == item.Id);

        if (existingItem != null)
        {
            // Update scalar properties
            context.Entry(existingItem).CurrentValues.SetValues(item);

            // Update BaseUnit relationship if changed
            // Ensure we don't try to create a new Unit if object is passed
            if (item.BaseUnit != null) context.Entry(item.BaseUnit).State = EntityState.Unchanged;

            // Handle ItemUnits Collection (Add/Update/Delete)
            // 1. Delete removed units
            foreach (var existingUnit in existingItem.ItemUnits.ToList())
            {
                if (!item.ItemUnits.Any(u => u.Id == existingUnit.Id && u.Id != 0))
                {
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
                    if (unit.Unit != null) context.Entry(unit.Unit).State = EntityState.Unchanged;
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
}
