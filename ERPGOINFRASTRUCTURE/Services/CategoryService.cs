using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class CategoryService : ICategoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CategoryService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Categories
            .Include(c => c.DefaultUnit)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return category;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Categories.Update(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var category = await context.Categories.FindAsync(id);
        if (category != null)
        {
            category.IsActive = false; // Soft delete
            context.Categories.Update(category);
            await context.SaveChangesAsync();
        }
    }


}
