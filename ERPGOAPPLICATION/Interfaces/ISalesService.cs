using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface ISalesService
{
    Task<SalesInvoice> CreateInvoice(SalesInvoice invoice);
}
