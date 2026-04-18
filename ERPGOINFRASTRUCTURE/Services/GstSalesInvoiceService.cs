using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class GstSalesInvoiceService : IGstSalesInvoiceService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public GstSalesInvoiceService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GstSalesInvoice> CreateInvoice(GstSalesInvoice invoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 0. Generate Invoice Number
                var prefixSetting    = await context.AppSettings.FindAsync("GstSalesInvoicePrefix");
                var nextNumSetting   = await context.AppSettings.FindAsync("GstSalesInvoiceNextNumber");
                var paddingSetting   = await context.AppSettings.FindAsync("GstSalesInvoicePadding");

                string prefix = prefixSetting?.Value  ?? "GINV";
                int nextNum   = int.Parse(nextNumSetting?.Value ?? "1");
                int padding   = int.Parse(paddingSetting?.Value ?? "3");

                invoice.InvoiceNo = $"{prefix}{nextNum.ToString("D" + padding)}";

                if (nextNumSetting == null)
                {
                    context.AppSettings.Add(new AppSetting { Key = "GstSalesInvoiceNextNumber", Value = (nextNum + 1).ToString() });
                }
                else
                {
                    nextNumSetting.Value = (nextNum + 1).ToString();
                    context.AppSettings.Update(nextNumSetting);
                }

                // 1. Recalculate GstAmount per item and TotalGstAmount (server-side safeguard)
                decimal totalGst = 0;
                foreach (var line in invoice.GstSalesInvoiceItems)
                {
                    line.GstAmount = Math.Round(line.Amount * line.GstPercent / 100m, 2);
                    totalGst += line.GstAmount;
                    line.Item  = null;
                    line.Unit  = null;
                    line.ItemUnit = null;
                }
                invoice.TotalGstAmount = totalGst;
                invoice.TotalAmount    = invoice.SubTotal - invoice.DiscountAmount + totalGst;

                // 2. Save Invoice header + lines
                context.GstSalesInvoices.Add(invoice);
                await context.SaveChangesAsync();

                // 3. Stock deduction + item CurrentStock update
                foreach (var line in invoice.GstSalesInvoiceItems)
                {
                    var stockEntry = new StockLedger
                    {
                        Date            = invoice.Date,
                        ItemId          = line.ItemId,
                        Qty             = -line.QtyInBaseUnit,
                        TransactionType = "GST_SALES",
                        RefId           = invoice.InvoiceNo
                    };
                    context.StockLedger.Add(stockEntry);

                    var item = await context.Items.FindAsync(line.ItemId);
                    if (item != null)
                    {
                        item.CurrentStock += stockEntry.Qty;
                        context.Items.Update(item);
                    }
                }

                // 4. Accounting: Debit Customer, Credit Sales, Credit Output GST
                var customer     = await context.Customers.FindAsync(invoice.CustomerId);
                var salesAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Sales Account" && a.IsDefault);

                // Auto-create "Output GST" account if missing
                var outputGstAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Output GST");
                if (outputGstAccount == null)
                {
                    // Find Current Liabilities head
                    var accountHead = await context.AccountHeads.FirstOrDefaultAsync(h => h.Name == "Current Liabilities")
                                   ?? await context.AccountHeads.FirstOrDefaultAsync();
                    
                    if (accountHead != null)
                    {
                        outputGstAccount = new Account
                        {
                            Name = "Output GST",
                            AccountHeadId = accountHead.Id,
                            IsDefault = false
                        };
                        context.Accounts.Add(outputGstAccount);
                        await context.SaveChangesAsync(); // get the Id
                    }
                }

                if (customer?.AccountId != null && salesAccount != null && outputGstAccount != null)
                {
                    decimal taxableAmount = invoice.SubTotal - invoice.DiscountAmount;
                    var je = new JournalEntry
                    {
                        VoucherDate    = invoice.Date,
                        VoucherNo      = invoice.InvoiceNo,
                        VoucherType    = "GstSales",
                        ReferenceId    = invoice.Id,
                        Narration      = $"GST Sales Invoice {invoice.InvoiceNo}",
                        JournalEntryLines = new List<JournalEntryLine>
                        {
                            // Debit Customer for full amount (including GST)
                            new JournalEntryLine { AccountId = customer.AccountId.Value, Debit = invoice.TotalAmount, Credit = 0 },
                            // Credit Sales for taxable amount
                            new JournalEntryLine { AccountId = salesAccount.Id, Debit = 0, Credit = taxableAmount },
                            // Credit Output GST
                            new JournalEntryLine { AccountId = outputGstAccount.Id, Debit = 0, Credit = invoice.TotalGstAmount }
                        }
                    };
                    context.JournalEntries.Add(je);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
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
        var prefixSetting  = await context.AppSettings.FindAsync("GstSalesInvoicePrefix");
        var nextNumSetting = await context.AppSettings.FindAsync("GstSalesInvoiceNextNumber");
        var paddingSetting = await context.AppSettings.FindAsync("GstSalesInvoicePadding");

        string prefix = prefixSetting?.Value  ?? "GINV";
        int nextNum   = int.Parse(nextNumSetting?.Value ?? "1");
        int padding   = int.Parse(paddingSetting?.Value ?? "3");

        return $"{prefix}{nextNum.ToString("D" + padding)}";
    }

    public async Task<List<GstSalesInvoice>> GetInvoicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GstSalesInvoices
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Customer)
            .Include(i => i.GstSalesInvoiceItems).ThenInclude(x => x.Item)
            .Include(i => i.GstSalesInvoiceItems).ThenInclude(x => x.Unit)
            .Include(i => i.GstSalesInvoiceItems).ThenInclude(x => x.ItemUnit)
            .OrderByDescending(i => i.Date)
            .ThenByDescending(i => i.Id)
            .ToListAsync();
    }

    public async Task<GstSalesInvoice?> GetInvoiceByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GstSalesInvoices
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Customer)
            .Include(i => i.GstSalesInvoiceItems).ThenInclude(x => x.Item)
            .Include(i => i.GstSalesInvoiceItems).ThenInclude(x => x.Unit)
            .Include(i => i.GstSalesInvoiceItems).ThenInclude(x => x.ItemUnit)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<GstSalesInvoice> UpdateInvoice(GstSalesInvoice invoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var existing = await context.GstSalesInvoices
            .Include(i => i.GstSalesInvoiceItems)
            .FirstOrDefaultAsync(i => i.Id == invoice.Id)
            ?? throw new Exception($"GST Invoice {invoice.Id} not found.");

        if (existing.Status == InvoiceStatus.Posted)
            throw new InvalidOperationException(
                $"Invoice {existing.InvoiceNo} is already Posted and cannot be edited. Raise a Credit Note to make corrections.");

        // Reverse old stock
        foreach (var oldLine in existing.GstSalesInvoiceItems)
        {
            var item = await context.Items.FindAsync(oldLine.ItemId);
            if (item != null)
            {
                item.CurrentStock += oldLine.QtyInBaseUnit;
                context.Entry(item).State = EntityState.Modified;
            }
        }
        context.GstSalesInvoiceItems.RemoveRange(existing.GstSalesInvoiceItems);

        // Update header
        existing.Date            = invoice.Date;
        existing.DueDate         = invoice.DueDate;
        existing.PaymentTerms    = invoice.PaymentTerms;
        existing.CustomerId      = invoice.CustomerId;
        existing.Notes           = invoice.Notes;
        existing.SubTotal        = invoice.SubTotal;
        existing.DiscountPercent = invoice.DiscountPercent;
        existing.DiscountAmount  = invoice.DiscountAmount;
        existing.TotalGstAmount  = invoice.TotalGstAmount;
        existing.TotalAmount     = invoice.TotalAmount;

        // Add new lines and apply stock
        foreach (var newLine in invoice.GstSalesInvoiceItems)
        {
            newLine.GstAmount = Math.Round(newLine.Amount * newLine.GstPercent / 100m, 2);
            var lineEntity = new GstSalesInvoiceItem
            {
                GstSalesInvoiceId = existing.Id,
                ItemId            = newLine.ItemId,
                UnitId            = newLine.UnitId,
                ItemUnitId        = newLine.ItemUnitId,
                Qty               = newLine.Qty,
                QtyPerBaseUnit    = newLine.QtyPerBaseUnit,
                QtyInBaseUnit     = newLine.QtyInBaseUnit,
                Rate              = newLine.Rate,
                DiscountPercent   = newLine.DiscountPercent,
                DiscountAmount    = newLine.DiscountAmount,
                Amount            = newLine.Amount,
                GstPercent        = newLine.GstPercent,
                GstAmount         = newLine.GstAmount
            };
            context.GstSalesInvoiceItems.Add(lineEntity);

            var item = await context.Items.FindAsync(newLine.ItemId);
            if (item != null)
            {
                item.CurrentStock -= newLine.QtyInBaseUnit;
                context.Entry(item).State = EntityState.Modified;
            }
        }

        // Update Journal Entry
        var oldJe = await context.JournalEntries.Include(j => j.JournalEntryLines)
            .FirstOrDefaultAsync(j => j.ReferenceId == existing.Id && j.VoucherType == "GstSales");
        if (oldJe != null) context.JournalEntries.Remove(oldJe);

        var customer     = await context.Customers.FindAsync(invoice.CustomerId);
        var salesAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Sales Account" && a.IsDefault);
        var gstAccount   = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Output GST");

        if (customer?.AccountId != null && salesAccount != null && gstAccount != null)
        {
            decimal taxable = invoice.SubTotal - invoice.DiscountAmount;
            context.JournalEntries.Add(new JournalEntry
            {
                VoucherDate = invoice.Date,
                VoucherNo   = existing.InvoiceNo,
                VoucherType = "GstSales",
                ReferenceId = existing.Id,
                Narration   = $"GST Sales Invoice {existing.InvoiceNo} (Updated)",
                JournalEntryLines = new List<JournalEntryLine>
                {
                    new JournalEntryLine { AccountId = customer.AccountId.Value, Debit = invoice.TotalAmount, Credit = 0 },
                    new JournalEntryLine { AccountId = salesAccount.Id, Debit = 0, Credit = taxable },
                    new JournalEntryLine { AccountId = gstAccount.Id,   Debit = 0, Credit = invoice.TotalGstAmount }
                }
            });
        }

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteInvoiceAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.GstSalesInvoices.Include(i => i.GstSalesInvoiceItems)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new Exception($"GST Invoice {id} not found.");

        if (invoice.Status == InvoiceStatus.Posted)
            throw new InvalidOperationException(
                $"Invoice {invoice.InvoiceNo} is Posted and cannot be deleted.");

        // Reverse stock
        foreach (var line in invoice.GstSalesInvoiceItems)
        {
            var item = await context.Items.FindAsync(line.ItemId);
            if (item != null)
            {
                item.CurrentStock += line.QtyInBaseUnit;
                context.Items.Update(item);
            }
        }

        // Remove journal entry
        var je = await context.JournalEntries.Include(j => j.JournalEntryLines)
            .FirstOrDefaultAsync(j => j.ReferenceId == id && j.VoucherType == "GstSales");
        if (je != null) context.JournalEntries.Remove(je);

        context.GstSalesInvoices.Remove(invoice);
        await context.SaveChangesAsync();
    }

    public async Task<GstSalesInvoice> PostInvoiceAsync(int invoiceId, string postedBy = "System")
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var invoice = await context.GstSalesInvoices
            .Include(i => i.GstSalesInvoiceItems)
            .FirstOrDefaultAsync(i => i.Id == invoiceId)
            ?? throw new Exception($"GST Invoice {invoiceId} not found.");

        if (invoice.Status == InvoiceStatus.Posted)
            throw new InvalidOperationException(
                $"Invoice {invoice.InvoiceNo} is already Posted.");

        if (!invoice.GstSalesInvoiceItems.Any())
            throw new InvalidOperationException(
                "Cannot post an invoice with no items.");

        invoice.Status   = InvoiceStatus.Posted;
        invoice.PostedAt = DateTime.Now;
        invoice.PostedBy = postedBy;

        await context.SaveChangesAsync();
        return invoice;
    }
}
