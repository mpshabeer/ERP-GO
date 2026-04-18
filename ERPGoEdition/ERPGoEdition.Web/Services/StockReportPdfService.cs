using ERPGOAPPLICATION.Interfaces;
using ERPGoEdition.Shared.Services;
using ERPGODomain.DTOs;
using QuestPDF.Fluent;

namespace ERPGoEdition.Web.Services;

public class StockReportPdfService : IStockReportPdfService
{
    private readonly IStockService _stockService;
    private readonly IAppSettingsService _appSettingsService;

    public StockReportPdfService(IStockService stockService, IAppSettingsService appSettingsService)
    {
        _stockService = stockService;
        _appSettingsService = appSettingsService;
    }

    public async Task<byte[]> GenerateStockReportPdfAsync(StockReportRequest request)
    {
        var reportData = await _stockService.GetStockReportAsync(request);
        var appSettings = await _appSettingsService.GetSettingsAsync();

        var document = new StockReportDocument(request, reportData, appSettings);
        return document.GeneratePdf();
    }
}
