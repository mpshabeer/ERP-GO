using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface IGstSalesInvoiceService
{
    Task<GstSalesInvoice> CreateInvoice(GstSalesInvoice invoice);
    Task<string> GetNextInvoiceNumber();
    Task<List<GstSalesInvoice>> GetInvoicesAsync();
    Task<GstSalesInvoice?> GetInvoiceByIdAsync(int id);
    Task<GstSalesInvoice> UpdateInvoice(GstSalesInvoice invoice);
    Task DeleteInvoiceAsync(int id);

    /// <summary>Finalises (posts) a draft invoice. Once posted the invoice cannot be edited or deleted.</summary>
    Task<GstSalesInvoice> PostInvoiceAsync(int invoiceId, string postedBy = "System");
}

