using QuestPDF.Fluent;
using ERPGODomain.DTOs;
using ERPGOAPPLICATION.Interfaces;
using ERPGoEdition.Shared.Services;

namespace ERPGoEdition.Web.Services;

public class ReportPdfService : IReportPdfService
{
    private readonly IAppSettingsService _appSettingsService;

    public ReportPdfService(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService;
    }

    public async Task<byte[]> GenerateSalesReportAsync(SalesReportRequest request, List<ItemWiseReportLine> lines)
    {
        var settings = await _appSettingsService.GetSettingsAsync();
        var document = new SalesItemReportDocument(request, lines, settings);
        return document.GeneratePdf();
    }

    public async Task<byte[]> GeneratePurchaseReportAsync(PurchaseReportRequest request, List<ItemWiseReportLine> lines)
    {
        var settings = await _appSettingsService.GetSettingsAsync();
        var document = new PurchaseItemReportDocument(request, lines, settings);
        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateAccountLedgerPdfAsync(LedgerReportResult result)
    {
        var settings = await _appSettingsService.GetSettingsAsync();
        var document = new AccountLedgerDocument(result, settings);
        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateTrialBalancePdfAsync(TrialBalanceResult result)
    {
        var settings = await _appSettingsService.GetSettingsAsync();
        var document = new TrialBalanceDocument(result, settings);
        return document.GeneratePdf();
    }
}
