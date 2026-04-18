using Refit;

namespace ERPGoEdition.Shared.Services;

public interface IReportApiClient
{
    /// <summary>Download a Sales Invoice as a PDF file.</summary>
    [Get("/api/reports/sales/invoice/{id}/pdf")]
    Task<HttpResponseMessage> GetSalesInvoicePdfAsync(int id);

    /// <summary>Download a GST Sales Invoice as a PDF file.</summary>
    [Get("/api/reports/gstsales/invoice/{id}/pdf")]
    Task<HttpResponseMessage> GetGstSalesInvoicePdfAsync(int id);
}
