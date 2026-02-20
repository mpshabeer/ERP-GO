using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface IAppSettingsService
{
    Task<string> GetSettingAsync(string key, string defaultValue = "");
    Task SetSettingAsync(string key, string value);
    Task<AppSettingsModel> GetSettingsAsync();
    Task SaveSettingsAsync(AppSettingsModel settings);
}

public class AppSettingsModel
{
    public bool AutoGenerateItemCode { get; set; } = true;
    public string ItemCodePrefix { get; set; } = "ITEM-";
    public string CategoryCodePrefix { get; set; } = "CAT-";
    public string SubCategoryCodePrefix { get; set; } = "SUB-";
    public int ItemCodeStartNumber { get; set; } = 1000;
}
