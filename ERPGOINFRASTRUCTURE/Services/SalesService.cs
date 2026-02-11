using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class SalesService : ISalesService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public SalesService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<SalesInvoice> CreateInvoice(SalesInvoice invoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // 1. Add Invoice Header
            context.SalesInvoices.Add(invoice);
            await context.SaveChangesAsync(); // Generates Invoice Id

            // 2. Process Items
            foreach (var line in invoice.SalesInvoiceItems)
            {
                line.SalesInvoiceId = invoice.Id;
                // Calculations should ideally be done before coming here, but we ensure consistency
                // QtyInBaseUnit is calculated: Qty * ConversionFactor.
                // We assume the UI or Controller populates QtyInBaseUnit correctly based on Unit Selection.
                
                context.SalesInvoiceItems.Add(line);

                // 3. Stock Impact
                var stockEntry = new StockLedger
                {
                    Date = invoice.Date,
                    ItemId = line.ItemId,
                    Qty = -line.QtyInBaseUnit, // Negative for Sales
                    TransactionType = "SALES",
                    RefId = invoice.InvoiceNo
                };
                context.StockLedger.Add(stockEntry);

                // 4. Link Item Update
                // It is safer to fetch and update the item within this same context/transaction
                var item = await context.Items.FindAsync(line.ItemId);
                if (item != null)
                {
                    item.CurrentStock += stockEntry.Qty; // Adding negative amount
                    context.Items.Update(item);
                }
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return invoice;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
