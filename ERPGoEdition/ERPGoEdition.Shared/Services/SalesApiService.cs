using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.DTOs;
using ERPGODomain.Entities;

namespace ERPGoEdition.Shared.Services;

public class SalesApiService : ISalesService
{
    private readonly ISalesApiClient _apiClient;

    public SalesApiService(ISalesApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<SalesInvoice> CreateInvoice(SalesInvoice invoice)
    {
        return await _apiClient.CreateInvoice(invoice);
    }

    public async Task<string> GetNextInvoiceNumber()
    {
        return await _apiClient.GetNextInvoiceNumber();
    }

    public async Task<PagedResult<SalesInvoice>> GetInvoicesAsync(InvoiceSearchRequest request)
    {
        return await _apiClient.GetInvoicesAsync(
            request.InvoiceNo,
            request.CustomerName,
            request.DateFrom,
            request.DateTo,
            request.Page,
            request.PageSize);
    }

    public async Task<SalesInvoice> UpdateInvoice(SalesInvoice invoice)
    {
        return await _apiClient.UpdateInvoice(invoice);
    }

    public Task<List<ItemWiseReportLine>> GetSalesReportAsync(SalesReportRequest request)
    {
        throw new NotImplementedException();
    }
}
