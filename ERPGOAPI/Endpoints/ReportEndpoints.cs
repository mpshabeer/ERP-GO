using ERPGOAPI.Reports;
using ERPGOAPPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace ERPGOAPI.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports");

        group.MapGet("/sales/invoice/{id}/pdf", async (int id, ISalesService salesService) =>
        {
            var request = new ERPGODomain.DTOs.InvoiceSearchRequest { Page = 1, PageSize = 9999 };
            var result = await salesService.GetInvoicesAsync(request);
            var invoice = result.Items.FirstOrDefault(x => x.Id == id);

            if (invoice is null)
                return Results.NotFound($"Invoice #{id} not found.");

            var document = new SalesInvoiceDocument(invoice);
            var pdfBytes = document.GeneratePdf();

            return Results.File(
                pdfBytes,
                contentType: "application/pdf",
                fileDownloadName: $"Invoice_{invoice.InvoiceNo}.pdf");
        })
        .WithName("GetSalesInvoicePdf");

        group.MapGet("/gstsales/invoice/{id}/pdf", async (int id, IGstSalesInvoiceService gstService) =>
        {
            var invoice = await gstService.GetInvoiceByIdAsync(id);
            if (invoice is null)
                return Results.NotFound($"GST Invoice #{id} not found.");

            var document = new ERPGoEdition.Shared.Reports.GstSalesInvoiceDocument(invoice);
            var pdfBytes = document.GeneratePdf();

            return Results.File(
                pdfBytes,
                contentType: "application/pdf",
                fileDownloadName: $"GstInvoice_{invoice.InvoiceNo}.pdf");
        })
        .WithName("GetGstSalesInvoicePdf");
    }
}
