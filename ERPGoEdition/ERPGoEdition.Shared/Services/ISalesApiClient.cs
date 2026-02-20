using ERPGODomain.DTOs;
using ERPGODomain.Entities;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface ISalesApiClient
{
    [Post("/api/sales/invoice")]
    Task<SalesInvoice> CreateInvoice([Body] SalesInvoice invoice);

    [Get("/api/sales/invoice/next-number")]
    Task<string> GetNextInvoiceNumber();

    [Get("/api/sales/invoices")]
    Task<PagedResult<SalesInvoice>> GetInvoicesAsync(
        [AliasAs("invoiceNo")] string? invoiceNo,
        [AliasAs("customerName")] string? customerName,
        [AliasAs("dateFrom")] DateTime? dateFrom,
        [AliasAs("dateTo")] DateTime? dateTo,
        [AliasAs("page")] int page,
        [AliasAs("pageSize")] int pageSize);

    [Put("/api/sales/invoice")]
    Task<SalesInvoice> UpdateInvoice([Body] SalesInvoice invoice);
}
