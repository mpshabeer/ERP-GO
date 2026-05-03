using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ERPGoEdition.Shared.Services;
using ERPGODomain.DTOs;

namespace ERPGoEdition.Services;

/// <summary>
/// Fetches PDFs directly from the ERPGOAPI backend.
/// Used on Mobile platforms (Android/iOS) where native QuestPDF layout generation is unsupported.
/// </summary>
public class RemotePdfService : IInvoicePdfService, IPurchasePdfService, IStockReportPdfService, IReportPdfService
{
    private readonly HttpClient _httpClient;

    public RemotePdfService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<byte[]> GenerateSalesInvoicePdfAsync(int invoiceId)
    {
        var response = await _httpClient.GetAsync($"/api/reports/sales/invoice/{invoiceId}/pdf");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<byte[]> GenerateGstSalesInvoicePdfAsync(int invoiceId)
    {
        var response = await _httpClient.GetAsync($"/api/reports/gstsales/invoice/{invoiceId}/pdf");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    // Remaining report methods throw NotSupported for now until API endpoints exist
    
    public Task<byte[]> GeneratePurchaseInvoicePdfAsync(int purchaseId)
        => throw new NotSupportedException("PDF Generation for Purchase Invoice is not routed on the API yet.");

    public Task<byte[]> GenerateStockReportPdfAsync(StockReportRequest request)
        => throw new NotSupportedException("PDF Generation for Stock Report is not routed on the API yet.");

    public Task<byte[]> GenerateSalesReportAsync(SalesReportRequest request, List<ItemWiseReportLine> lines)
        => throw new NotSupportedException("PDF Generation for Sales Report is not routed on the API yet.");
        
    public Task<byte[]> GeneratePurchaseReportAsync(PurchaseReportRequest request, List<ItemWiseReportLine> lines)
        => throw new NotSupportedException("PDF Generation for Purchase Report is not routed on the API yet.");

    public Task<byte[]> GenerateAccountLedgerPdfAsync(LedgerReportResult result)
        => throw new NotSupportedException("PDF Generation for Account Ledger is not routed on the API yet.");
        
    public Task<byte[]> GenerateTrialBalancePdfAsync(TrialBalanceResult result)
        => throw new NotSupportedException("PDF Generation for Trial Balance is not routed on the API yet.");
}
