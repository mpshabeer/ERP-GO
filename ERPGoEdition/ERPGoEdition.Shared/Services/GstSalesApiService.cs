using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;

namespace ERPGoEdition.Shared.Services;

/// <summary>
/// Blazor-side service implementation that delegates to the Refit API client.
/// Registered as IGstSalesInvoiceService in the Shared DI container.
/// </summary>
public class GstSalesApiService : IGstSalesInvoiceService
{
    private readonly IGstSalesApiClient _client;

    public GstSalesApiService(IGstSalesApiClient client)
    {
        _client = client;
    }

    public Task<GstSalesInvoice> CreateInvoice(GstSalesInvoice invoice)
        => _client.CreateInvoice(invoice);

    public Task<string> GetNextInvoiceNumber()
        => _client.GetNextInvoiceNumber();

    public Task<List<GstSalesInvoice>> GetInvoicesAsync()
        => _client.GetInvoicesAsync();

    public Task<GstSalesInvoice?> GetInvoiceByIdAsync(int id)
        => _client.GetInvoiceByIdAsync(id)!;

    public Task<GstSalesInvoice> UpdateInvoice(GstSalesInvoice invoice)
        => _client.UpdateInvoice(invoice);

    public Task DeleteInvoiceAsync(int id)
        => _client.DeleteInvoiceAsync(id);

    public Task<GstSalesInvoice> PostInvoiceAsync(int invoiceId, string postedBy = "System")
        => _client.PostInvoiceAsync(invoiceId, postedBy);
}
