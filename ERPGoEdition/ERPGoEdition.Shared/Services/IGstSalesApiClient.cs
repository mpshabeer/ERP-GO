using ERPGODomain.Entities;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface IGstSalesApiClient
{
    [Post("/api/gstsales/invoice")]
    Task<GstSalesInvoice> CreateInvoice([Body] GstSalesInvoice invoice);

    [Get("/api/gstsales/invoice/next-number")]
    Task<string> GetNextInvoiceNumber();

    [Get("/api/gstsales/invoices")]
    Task<List<GstSalesInvoice>> GetInvoicesAsync();

    [Get("/api/gstsales/invoice/{id}")]
    Task<GstSalesInvoice> GetInvoiceByIdAsync(int id);

    [Put("/api/gstsales/invoice")]
    Task<GstSalesInvoice> UpdateInvoice([Body] GstSalesInvoice invoice);

    [Delete("/api/gstsales/invoice/{id}")]
    Task DeleteInvoiceAsync(int id);

    [Post("/api/gstsales/invoice/{id}/post")]
    Task<GstSalesInvoice> PostInvoiceAsync(int id, [Query] string postedBy = "User");
}
