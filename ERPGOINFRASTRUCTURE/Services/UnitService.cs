
using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class UnitService : IUnitService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public UnitService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Unit>> GetAllUnitsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        //return await context.Units.Where(u => u.IsActive).ToListAsync();
        return await context.Units.OrderByDescending(u=>u.IsActive).ToListAsync();

    }

    public async Task<List<Unit>> GetAllUnitsIncludeInactiveAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Units.ToListAsync();
    }

    public async Task<Unit?> GetUnitByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Units.FindAsync(id);
    }

    public async Task<Unit> AddUnitAsync(Unit unit)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Units.Add(unit);
            await context.SaveChangesAsync();
            return unit;
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<Unit> UpdateUnitAsync(Unit unit)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Units.Update(unit);
        await context.SaveChangesAsync();
        return unit;
    }

    public async Task DeleteUnitAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var unit = await context.Units.FindAsync(id);
        if (unit != null)
        {
            unit.IsActive = false; // Soft delete
            context.Units.Update(unit);
            await context.SaveChangesAsync();
        }
    }
}
