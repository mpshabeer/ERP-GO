using ERPGOAPPLICATION.Interfaces;
using ERPGoEdition.Shared.Services;
using ERPGODomain.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ERPGoEdition.Web.Services;

/// <summary>
/// Generates PDF invoices directly using ISalesService (no separate API call needed).
/// Only used in the Blazor Server / Web host — MAUI would need its own impl.
/// </summary>
public class InvoicePdfService : IInvoicePdfService
{
    private readonly ISalesService _salesService;
    private readonly IGstSalesInvoiceService _gstSalesService;
    private readonly IAppSettingsService _appSettingsService;

    public InvoicePdfService(
        ISalesService salesService, 
        IGstSalesInvoiceService gstSalesService, 
        IAppSettingsService appSettingsService)
    {
        _salesService = salesService;
        _gstSalesService = gstSalesService;
        _appSettingsService = appSettingsService;
        // Register QuestPDF community license once
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateSalesInvoicePdfAsync(int invoiceId)
    {
        // Fetch all invoices (the existing service method uses search/paging)
        var result = await _salesService.GetInvoicesAsync(new InvoiceSearchRequest
        {
            Page = 1,
            PageSize = 9999
        });

        var invoice = result.Items.FirstOrDefault(x => x.Id == invoiceId)
            ?? throw new KeyNotFoundException($"Invoice #{invoiceId} not found.");

        var settings = await _appSettingsService.GetSettingsAsync();

        var document = new SalesInvoiceDocument(invoice, settings);
        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateGstSalesInvoicePdfAsync(int invoiceId)
    {
        var invoice = await _gstSalesService.GetInvoiceByIdAsync(invoiceId)
            ?? throw new KeyNotFoundException($"GST Invoice #{invoiceId} not found.");

        var settings = await _appSettingsService.GetSettingsAsync();

        var document = new ERPGoEdition.Shared.Reports.GstSalesInvoiceDocument(
            invoice,
            companyName: settings.CompanyName,
            companyAddress: settings.CompanyAddress,
            companyGstin: settings.CompanyTaxNumber,
            companyPhone: settings.CompanyPhone,
            companyEmail: settings.CompanyEmail
        );

        return document.GeneratePdf();
    }
}
