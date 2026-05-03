using ERPGOAPPLICATION.Interfaces;
using ERPGoEdition.Shared.Services;
using ERPGODomain.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ERPGoEdition.Shared.Services;

public class PurchasePdfService : IPurchasePdfService
{
    private readonly IPurchaseService _purchaseService;
    private readonly IAppSettingsService _appSettingsService;

    public PurchasePdfService(IPurchaseService purchaseService, IAppSettingsService appSettingsService)
    {
        _purchaseService = purchaseService;
        _appSettingsService = appSettingsService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GeneratePurchaseInvoicePdfAsync(int purchaseId)
    {
        var result = await _purchaseService.GetPurchasesAsync(new PurchaseSearchRequest
        {
            Page = 1,
            PageSize = 9999
        });

        var purchase = result.Items.FirstOrDefault(x => x.Id == purchaseId)
            ?? throw new KeyNotFoundException($"Purchase #{purchaseId} not found.");

        var settings = await _appSettingsService.GetSettingsAsync();

        var document = new PurchaseInvoiceDocument(purchase, settings);
        return document.GeneratePdf();
    }
}
