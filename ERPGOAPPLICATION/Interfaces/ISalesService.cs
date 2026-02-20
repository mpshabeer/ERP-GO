using ERPGODomain.DTOs;
using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface ISalesService
{
    Task<SalesInvoice> CreateInvoice(SalesInvoice invoice);
    Task<string> GetNextInvoiceNumber();
    Task<PagedResult<SalesInvoice>> GetInvoicesAsync(InvoiceSearchRequest request);
    Task<SalesInvoice> UpdateInvoice(SalesInvoice invoice);
}
