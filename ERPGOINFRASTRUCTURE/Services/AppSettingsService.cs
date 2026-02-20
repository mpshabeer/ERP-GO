using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class AppSettingsService : IAppSettingsService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    
    // Static cache
    private static AppSettingsModel? _cachedSettings;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public AppSettingsService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<string> GetSettingAsync(string key, string defaultValue = "")
    {
        var settings = await GetSettingsAsync();
        return key switch
        {
            "AutoGenerateItemCode" => settings.AutoGenerateItemCode.ToString(),
            "ItemCodePrefix" => settings.ItemCodePrefix,
            "CategoryCodePrefix" => settings.CategoryCodePrefix,
            "SubCategoryCodePrefix" => settings.SubCategoryCodePrefix,
            _ => defaultValue
        };
    }

    public async Task SetSettingAsync(string key, string value)
    {
        // For individual setting updates, we just save the whole model to keep cache in sync or invalidate it.
        // But for this implementation, we primarily use SaveSettingsAsync with the full model.
        // Implementing this for compatibility if needed, but updating cache is key.
        
        using var context = await _contextFactory.CreateDbContextAsync();
        var setting = await context.AppSettings.FindAsync(key);
        if (setting == null)
        {
            context.AppSettings.Add(new AppSetting { Key = key, Value = value });
        }
        else
        {
            setting.Value = value;
        }
        await context.SaveChangesAsync();
        
        // Invalidate cache so next read fetches fresh data
        _cachedSettings = null; 
    }

    public async Task<AppSettingsModel> GetSettingsAsync()
    {
        if (_cachedSettings != null) return _cachedSettings;

        try
        {
            await _semaphore.WaitAsync();
            if (_cachedSettings != null) return _cachedSettings;

            try 
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var settings = await context.AppSettings.ToListAsync();
                
                _cachedSettings = new AppSettingsModel
                {
                    AutoGenerateItemCode = bool.Parse(settings.FirstOrDefault(s => s.Key == "AutoGenerateItemCode")?.Value ?? "true"),
                    ItemCodePrefix = settings.FirstOrDefault(s => s.Key == "ItemCodePrefix")?.Value ?? "ITEM-",
                    CategoryCodePrefix = settings.FirstOrDefault(s => s.Key == "CategoryCodePrefix")?.Value ?? "CAT-",
                    SubCategoryCodePrefix = settings.FirstOrDefault(s => s.Key == "SubCategoryCodePrefix")?.Value ?? "SUB-",
                    ItemCodeStartNumber = int.Parse(settings.FirstOrDefault(s => s.Key == "ItemCodeStartNumber")?.Value ?? "1000")
                };
            }
            catch (Exception)
            {
                // Fallback if table missing or DB error
                _cachedSettings = new AppSettingsModel();
            }
            
            return _cachedSettings;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveSettingsAsync(AppSettingsModel model)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        var keys = new Dictionary<string, string>
        {
            { "AutoGenerateItemCode", model.AutoGenerateItemCode.ToString() },
            { "ItemCodePrefix", model.ItemCodePrefix },
            { "CategoryCodePrefix", model.CategoryCodePrefix },
            { "SubCategoryCodePrefix", model.SubCategoryCodePrefix },
            { "ItemCodeStartNumber", model.ItemCodeStartNumber.ToString() }
        };

        foreach (var kvp in keys)
        {
            var setting = await context.AppSettings.FindAsync(kvp.Key);
            if (setting == null)
            {
                context.AppSettings.Add(new AppSetting { Key = kvp.Key, Value = kvp.Value });
            }
            else
            {
                setting.Value = kvp.Value;
            }
        }
        await context.SaveChangesAsync();
        
        // Update cache
        _cachedSettings = model;
    }
}
