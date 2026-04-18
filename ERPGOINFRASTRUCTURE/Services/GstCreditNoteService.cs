using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class GstCreditNoteService : IGstCreditNoteService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public GstCreditNoteService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<string> GetNextCreditNoteNumber()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var prefixSetting    = await context.AppSettings.FindAsync("GstCreditNotePrefix");
        var nextNumSetting   = await context.AppSettings.FindAsync("GstCreditNoteNextNumber");
        var paddingSetting   = await context.AppSettings.FindAsync("GstCreditNotePadding");

        string prefix = prefixSetting?.Value   ?? "CNV";
        int    next   = int.Parse(nextNumSetting?.Value ?? "1");
        int    pad    = int.Parse(paddingSetting?.Value ?? "3");

        return $"{prefix}{next.ToString("D" + pad)}";
    }

    public async Task<GstCreditNote> CreateCreditNoteAsync(GstCreditNote note)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var tx = await context.Database.BeginTransactionAsync();
            try
            {
                // ── Auto-generate Credit Note number ─────────────
                var prefixSetting  = await context.AppSettings.FindAsync("GstCreditNotePrefix");
                var nextNumSetting = await context.AppSettings.FindAsync("GstCreditNoteNextNumber");
                var paddingSetting = await context.AppSettings.FindAsync("GstCreditNotePadding");

                string prefix = prefixSetting?.Value   ?? "CNV";
                int    next   = int.Parse(nextNumSetting?.Value ?? "1");
                int    pad    = int.Parse(paddingSetting?.Value ?? "3");

                note.CreditNoteNo = $"{prefix}{next.ToString("D" + pad)}";

                if (nextNumSetting == null)
                    context.AppSettings.Add(new AppSetting { Key = "GstCreditNoteNextNumber", Value = (next + 1).ToString() });
                else
                {
                    nextNumSetting.Value = (next + 1).ToString();
                    context.AppSettings.Update(nextNumSetting);
                }

                // ── Recalculate server-side ───────────────────────
                decimal totalGst = 0;
                foreach (var item in note.Items)
                {
                    item.GstAmount = Math.Round(item.Amount * item.GstPercent / 100m, 2);
                    totalGst      += item.GstAmount;
                    item.Item      = null;
                    item.Unit      = null;
                }
                note.TotalGstAmount = totalGst;
                note.TotalAmount    = note.SubTotal + totalGst;
                note.CreatedAt      = DateTime.Now;

                // Detach nav props to avoid EF tracking issues
                note.OriginalInvoice = null;
                note.Customer        = null;

                context.GstCreditNotes.Add(note);
                await context.SaveChangesAsync();

                // ── Accounting — reverse the original sale ────────
                var customer      = await context.Customers.FindAsync(note.CustomerId);
                var salesAccount  = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Sales Account" && a.IsDefault);
                var gstAccount    = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Output GST");

                if (customer?.AccountId != null && salesAccount != null && gstAccount != null)
                {
                    var je = new JournalEntry
                    {
                        VoucherDate = note.Date,
                        VoucherNo   = note.CreditNoteNo,
                        VoucherType = "GstCreditNote",
                        ReferenceId = note.Id,
                        Narration   = $"Credit Note {note.CreditNoteNo} against {GetOriginalInvoiceNo(context, note.OriginalInvoiceId)}",
                        JournalEntryLines = new List<JournalEntryLine>
                        {
                            // Debit Sales (reversal)
                            new JournalEntryLine { AccountId = salesAccount.Id,       Debit = note.SubTotal,          Credit = 0 },
                            // Debit Output GST (reversal)
                            new JournalEntryLine { AccountId = gstAccount.Id,          Debit = note.TotalGstAmount,    Credit = 0 },
                            // Credit Customer
                            new JournalEntryLine { AccountId = customer.AccountId.Value, Debit = 0, Credit = note.TotalAmount }
                        }
                    };
                    context.JournalEntries.Add(je);
                }

                await context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        });

        return note;
    }

    private static string GetOriginalInvoiceNo(ApplicationDbContext ctx, int id)
    {
        return ctx.GstSalesInvoices.Where(i => i.Id == id)
                  .Select(i => i.InvoiceNo).FirstOrDefault() ?? id.ToString();
    }

    public async Task<List<GstCreditNote>> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GstCreditNotes
            .AsNoTracking().AsSplitQuery()
            .Include(c => c.Customer)
            .Include(c => c.OriginalInvoice)
            .Include(c => c.Items).ThenInclude(i => i.Item)
            .Include(c => c.Items).ThenInclude(i => i.Unit)
            .OrderByDescending(c => c.Date).ThenByDescending(c => c.Id)
            .ToListAsync();
    }

    public async Task<GstCreditNote?> GetByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GstCreditNotes
            .AsNoTracking().AsSplitQuery()
            .Include(c => c.Customer)
            .Include(c => c.OriginalInvoice)
            .Include(c => c.Items).ThenInclude(i => i.Item)
            .Include(c => c.Items).ThenInclude(i => i.Unit)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<GstCreditNote>> GetByInvoiceIdAsync(int originalInvoiceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GstCreditNotes
            .AsNoTracking().AsSplitQuery()
            .Include(c => c.Customer)
            .Include(c => c.Items).ThenInclude(i => i.Item)
            .Include(c => c.Items).ThenInclude(i => i.Unit)
            .Where(c => c.OriginalInvoiceId == originalInvoiceId)
            .OrderByDescending(c => c.Date)
            .ToListAsync();
    }
}
