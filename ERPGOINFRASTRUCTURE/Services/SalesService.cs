using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.DTOs;
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
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 0. Generate Invoice Number (Transactional)
                var prefixSetting = await context.AppSettings.FindAsync("SalesInvoicePrefix");
                var nextNumSetting = await context.AppSettings.FindAsync("SalesInvoiceNextNumber");
                var paddingSetting = await context.AppSettings.FindAsync("SalesInvoicePadding");

                string prefix = prefixSetting?.Value ?? "INV";
                int nextNum = int.Parse(nextNumSetting?.Value ?? "1");
                int padding = int.Parse(paddingSetting?.Value ?? "3");

                invoice.InvoiceNo = $"{prefix}{nextNum.ToString("D" + padding)}";

                // Increment and Save
                if (nextNumSetting == null)
                {
                    context.AppSettings.Add(new AppSetting { Key = "SalesInvoiceNextNumber", Value = (nextNum + 1).ToString() });
                }
                else
                {
                    nextNumSetting.Value = (nextNum + 1).ToString();
                    context.AppSettings.Update(nextNumSetting);
                }
                
                // 1. Prepare Invoice Graph
                // Prevent EF from trying to re-insert the Master Items (Product)
                foreach(var line in invoice.SalesInvoiceItems) 
                {
                     line.Item = null; 
                     line.Unit = null;
                     line.ItemUnit = null;                }

                // 2. Add Invoice Header (and Children via Graph)
                context.SalesInvoices.Add(invoice);
                await context.SaveChangesAsync(); // Generates Invoice Id and Item Ids

                // 3. Process Stock Impact (Items are already saved)
                foreach (var line in invoice.SalesInvoiceItems)
                {
                    // line.SalesInvoiceId is already set by EF Fixup
                    // context.SalesInvoiceItems.Add(line); <--- REMOVED (Caused Identity Error)

                    // 4. Stock Impact
                    var stockEntry = new StockLedger
                    {
                        Date = invoice.Date,
                        ItemId = line.ItemId,
                        Qty = -line.QtyInBaseUnit, // Negative for Sales
                        TransactionType = "SALES",
                        RefId = invoice.InvoiceNo
                    };
                    context.StockLedger.Add(stockEntry);

                    // 5. Link Item Update
                    var item = await context.Items.FindAsync(line.ItemId);
                    if (item != null)
                    {
                        item.CurrentStock += stockEntry.Qty; // Adding negative amount
                        context.Items.Update(item);
                    }
                }

                // 6. Post Accounting Journal Entry
                var customer = await context.Customers.FindAsync(invoice.CustomerId);
                var salesAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Sales Account" && a.IsDefault);
                
                if (customer?.AccountId != null && salesAccount != null)
                {
                    var je = new JournalEntry
                    {
                        VoucherDate = invoice.Date,
                        VoucherNo = invoice.InvoiceNo,
                        VoucherType = "Sales",
                        ReferenceId = invoice.Id,
                        Narration = $"Sales Invoice {invoice.InvoiceNo}",
                        JournalEntryLines = new List<JournalEntryLine>
                        {
                            new JournalEntryLine { AccountId = customer.AccountId.Value, Debit = invoice.TotalAmount, Credit = 0 },
                            new JournalEntryLine { AccountId = salesAccount.Id, Debit = 0, Credit = invoice.TotalAmount }
                        }
                    };
                    context.JournalEntries.Add(je);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        return invoice;
    }

    public async Task<string> GetNextInvoiceNumber()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        // Just read, no transaction needed for preview
        var prefixSetting = await context.AppSettings.FindAsync("SalesInvoicePrefix");
        var nextNumSetting = await context.AppSettings.FindAsync("SalesInvoiceNextNumber");
        var paddingSetting = await context.AppSettings.FindAsync("SalesInvoicePadding");

        string prefix = prefixSetting?.Value ?? "INV";
        int nextNum = int.Parse(nextNumSetting?.Value ?? "1");
        int padding = int.Parse(paddingSetting?.Value ?? "3");

        return $"{prefix}{nextNum.ToString("D" + padding)}";
    }

    public async Task<PagedResult<SalesInvoice>> GetInvoicesAsync(InvoiceSearchRequest request)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.SalesInvoices
            .Include(i => i.Customer)
            .Include(i => i.SalesInvoiceItems)
                .ThenInclude(x => x.Unit)
            .Include(i => i.SalesInvoiceItems)
                .ThenInclude(x => x.Item)
                    .ThenInclude(it => it.BaseUnit)
            .Include(i => i.SalesInvoiceItems)
                .ThenInclude(x => x.Item)
                    .ThenInclude(it => it.ItemUnits)
                        .ThenInclude(iu => iu.Unit)
            .Include(i => i.SalesInvoiceItems)
                .ThenInclude(x => x.ItemUnit)
            .AsQueryable();

        // Filters
        if (!string.IsNullOrWhiteSpace(request.InvoiceNo))
            query = query.Where(i => i.InvoiceNo.Contains(request.InvoiceNo));

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
            query = query.Where(i => i.Customer != null && i.Customer.Name.Contains(request.CustomerName));

        if (request.DateFrom.HasValue)
            query = query.Where(i => i.Date >= request.DateFrom.Value.Date);

        if (request.DateTo.HasValue)
            query = query.Where(i => i.Date <= request.DateTo.Value.Date.AddDays(1).AddTicks(-1));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(i => i.Date)
            .ThenByDescending(i => i.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResult<SalesInvoice>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<SalesInvoice> UpdateInvoice(SalesInvoice invoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // 1. Fetch existing invoice with items
        var existingInvoice = await context.SalesInvoices
            .Include(i => i.SalesInvoiceItems)
            .FirstOrDefaultAsync(i => i.Id == invoice.Id);

        if (existingInvoice == null)
            throw new Exception($"Invoice {invoice.Id} not found.");

        // 2. Reverse Stock Impact of Old Items
        foreach (var oldLine in existingInvoice.SalesInvoiceItems)
        {
            var item = await context.Items.FindAsync(oldLine.ItemId);
            if (item != null)
            {
                // Add back the quantity sold
                item.CurrentStock += oldLine.QtyInBaseUnit;
                context.Entry(item).State = EntityState.Modified;
            }
        }

        // 3. Remove Old Lines
        context.SalesInvoiceItems.RemoveRange(existingInvoice.SalesInvoiceItems);

        // 4. Update Header Fields
        existingInvoice.Date = invoice.Date;
        existingInvoice.DueDate = invoice.DueDate;
        existingInvoice.PaymentTerms = invoice.PaymentTerms;
        existingInvoice.CustomerId = invoice.CustomerId;
        existingInvoice.Notes = invoice.Notes;
        existingInvoice.SubTotal = invoice.SubTotal;
        existingInvoice.DiscountPercent = invoice.DiscountPercent;
        existingInvoice.DiscountAmount = invoice.DiscountAmount;
        existingInvoice.TaxPercent = invoice.TaxPercent;
        existingInvoice.TaxAmount = invoice.TaxAmount;
        existingInvoice.TotalAmount = invoice.TotalAmount;

        // 5. Add New Lines & Apply New Stock Impact
        foreach (var newLine in invoice.SalesInvoiceItems)
        {
            // Create new entity for the line
            var lineEntity = new SalesInvoiceItem
            {
                SalesInvoiceId = existingInvoice.Id,
                ItemId = newLine.ItemId,
                UnitId = newLine.UnitId,
                ItemUnitId = newLine.ItemUnitId,
                Qty = newLine.Qty,
                Rate = newLine.Rate,
                DiscountPercent = newLine.DiscountPercent,
                DiscountAmount = newLine.DiscountAmount,
                Amount = newLine.Amount,
                QtyPerBaseUnit = newLine.QtyPerBaseUnit,
                QtyInBaseUnit = newLine.QtyInBaseUnit
            };

            context.SalesInvoiceItems.Add(lineEntity);

            // Deduct stock for the new line
            var item = await context.Items.FindAsync(newLine.ItemId);
            if (item != null)
            {
                item.CurrentStock -= newLine.QtyInBaseUnit;
                context.Entry(item).State = EntityState.Modified;
            }
        }

        // 6. Update Accounting Journal Entry
        var oldJe = await context.JournalEntries.Include(j => j.JournalEntryLines)
            .FirstOrDefaultAsync(j => j.ReferenceId == existingInvoice.Id && j.VoucherType == "Sales");
        if (oldJe != null)
        {
            context.JournalEntries.Remove(oldJe);
        }

        var customer = await context.Customers.FindAsync(invoice.CustomerId);
        var salesAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Sales Account" && a.IsDefault);
        
        if (customer?.AccountId != null && salesAccount != null)
        {
            var je = new JournalEntry
            {
                VoucherDate = invoice.Date,
                VoucherNo = existingInvoice.InvoiceNo,
                VoucherType = "Sales",
                ReferenceId = existingInvoice.Id,
                Narration = $"Sales Invoice {existingInvoice.InvoiceNo} (Updated)",
                JournalEntryLines = new List<JournalEntryLine>
                {
                    new JournalEntryLine { AccountId = customer.AccountId.Value, Debit = invoice.TotalAmount, Credit = 0 },
                    new JournalEntryLine { AccountId = salesAccount.Id, Debit = 0, Credit = invoice.TotalAmount }
                }
            };
            context.JournalEntries.Add(je);
        }

        await context.SaveChangesAsync();
        return existingInvoice;
    }

    public async Task<List<ItemWiseReportLine>> GetSalesReportAsync(SalesReportRequest request)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.SalesInvoiceItems
            .Include(i => i.SalesInvoice)
                .ThenInclude(inv => inv.Customer)
            .Include(i => i.Item)
                .ThenInclude(it => it.Category)
            .Include(i => i.Unit)
            .AsQueryable();

        if (request.CustomerId.HasValue)
            query = query.Where(i => i.SalesInvoice != null && i.SalesInvoice.CustomerId == request.CustomerId.Value);
            
        if (request.CategoryId.HasValue)
            query = query.Where(i => i.Item != null && i.Item.CategoryId == request.CategoryId.Value);

        if (request.ItemId.HasValue)
            query = query.Where(i => i.ItemId == request.ItemId.Value);

        query = query.Where(i => i.SalesInvoice != null && i.SalesInvoice.Date >= request.FromDate.Date);
        query = query.Where(i => i.SalesInvoice != null && i.SalesInvoice.Date <= request.ToDate.Date.AddDays(1).AddTicks(-1));

        var items = await query
            .OrderByDescending(i => i.SalesInvoice!.Date)
            .ThenByDescending(i => i.SalesInvoiceId)
            .ToListAsync();

        return items.Select(i => new ItemWiseReportLine
        {
            Date = i.SalesInvoice!.Date,
            DocumentNo = i.SalesInvoice.InvoiceNo,
            PartyName = i.SalesInvoice.Customer?.Name ?? "Unknown Customer",
            ItemId = i.ItemId,
            ItemCode = i.Item?.ItemCode ?? "",
            ItemName = i.Item?.Name ?? "",
            CategoryName = i.Item?.Category?.Name ?? "Uncategorized",
            UnitName = i.Unit?.Name ?? "Unit",
            Qty = i.Qty,
            QtyPerBaseUnit = i.QtyPerBaseUnit,
            Rate = i.Rate,
            Amount = i.Amount
        }).ToList();
    }
}
