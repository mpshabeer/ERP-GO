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

    // Company Information for Reports
    public string CompanyName { get; set; } = "ERP Go Edition";
    public string CompanyAddress { get; set; } = "Your Company Address, City, Country";
    public string CompanyPhone { get; set; } = "123-456-7890";
    public string CompanyEmail { get; set; } = "info@company.com";
    public string CompanyTaxNumber { get; set; } = "TAX-123456789";
    public string InvoiceFooterMessage { get; set; } = "Thank you for your business!";
}
