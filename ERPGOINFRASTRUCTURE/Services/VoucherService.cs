using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.DTOs;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class VoucherService : IVoucherService
{
    private readonly ApplicationDbContext _context;

    public VoucherService(ApplicationDbContext context)
    {
        _context = context;
    }

    // --- RECEIPTS ---
    public async Task<List<Receipt>> GetReceiptsAsync()
    {
        return await _context.Receipts
            .Include(r => r.CashBankAccount)
            .Include(r => r.PartyAccount)
            .OrderByDescending(r => r.Date)
            .ToListAsync();
    }

    public async Task<Receipt> CreateReceiptAsync(ReceiptRequest request)
    {
        var receipt = new Receipt
        {
            ReceiptNo = string.IsNullOrEmpty(request.ReceiptNo) ? await GenerateVoucherNoAsync("RC") : request.ReceiptNo,
            Date = request.Date,
            Amount = request.Amount,
            Narration = request.Narration,
            CashBankAccountId = request.CashBankAccountId,
            PartyAccountId = request.PartyAccountId
        };
        
        _context.Receipts.Add(receipt);

        // Receipt: Debit Cash/Bank, Credit Party (Customer/Income)
        var journalEntry = new JournalEntry
        {
            VoucherDate = request.Date,
            VoucherNo = receipt.ReceiptNo,
            VoucherType = "Receipt",
            Narration = request.Narration,
            JournalEntryLines = new List<JournalEntryLine>
            {
                new JournalEntryLine { AccountId = request.CashBankAccountId, Debit = request.Amount, Credit = 0 },
                new JournalEntryLine { AccountId = request.PartyAccountId, Debit = 0, Credit = request.Amount }
            }
        };
        _context.JournalEntries.Add(journalEntry);

        await _context.SaveChangesAsync();
        
        journalEntry.ReferenceId = receipt.Id;
        await _context.SaveChangesAsync();

        return receipt;
    }

    // --- PAYMENTS ---
    public async Task<List<Payment>> GetPaymentsAsync()
    {
        return await _context.Payments
            .Include(p => p.CashBankAccount)
            .Include(p => p.PartyAccount)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public async Task<Payment> CreatePaymentAsync(PaymentRequest request)
    {
        var payment = new Payment
        {
            PaymentNo = string.IsNullOrEmpty(request.PaymentNo) ? await GenerateVoucherNoAsync("PM") : request.PaymentNo,
            Date = request.Date,
            Amount = request.Amount,
            Narration = request.Narration,
            CashBankAccountId = request.CashBankAccountId,
            PartyAccountId = request.PartyAccountId
        };
        
        _context.Payments.Add(payment);

        // Payment: Debit Party (Supplier), Credit Cash/Bank
        var journalEntry = new JournalEntry
        {
            VoucherDate = request.Date,
            VoucherNo = payment.PaymentNo,
            VoucherType = "Payment",
            Narration = request.Narration,
            JournalEntryLines = new List<JournalEntryLine>
            {
                new JournalEntryLine { AccountId = request.PartyAccountId, Debit = request.Amount, Credit = 0 },
                new JournalEntryLine { AccountId = request.CashBankAccountId, Debit = 0, Credit = request.Amount }
            }
        };
        _context.JournalEntries.Add(journalEntry);

        await _context.SaveChangesAsync();
        
        journalEntry.ReferenceId = payment.Id;
        await _context.SaveChangesAsync();

        return payment;
    }

    // --- EXPENSES ---
    public async Task<List<Expense>> GetExpensesAsync()
    {
        return await _context.Expenses
            .Include(e => e.CashBankAccount)
            .Include(e => e.ExpenseAccount)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<Expense> CreateExpenseAsync(ExpenseRequest request)
    {
        var expense = new Expense
        {
            ExpenseNo = string.IsNullOrEmpty(request.ExpenseNo) ? await GenerateVoucherNoAsync("EX") : request.ExpenseNo,
            Date = request.Date,
            Amount = request.Amount,
            Narration = request.Narration,
            CashBankAccountId = request.CashBankAccountId,
            ExpenseAccountId = request.ExpenseAccountId
        };
        
        _context.Expenses.Add(expense);

        // Expense: Debit Expense Account, Credit Cash/Bank
        var journalEntry = new JournalEntry
        {
            VoucherDate = request.Date,
            VoucherNo = expense.ExpenseNo,
            VoucherType = "Expense",
            Narration = request.Narration,
            JournalEntryLines = new List<JournalEntryLine>
            {
                new JournalEntryLine { AccountId = request.ExpenseAccountId, Debit = request.Amount, Credit = 0 },
                new JournalEntryLine { AccountId = request.CashBankAccountId, Debit = 0, Credit = request.Amount }
            }
        };
        _context.JournalEntries.Add(journalEntry);

        await _context.SaveChangesAsync();
        
        journalEntry.ReferenceId = expense.Id;
        await _context.SaveChangesAsync();

        return expense;
    }

    private async Task<string> GenerateVoucherNoAsync(string prefix)
    {
        int lastId = 0;
        
        if (prefix == "RC")
        {
            lastId = await _context.Receipts.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync();
        }
        else if (prefix == "PM")
        {
            lastId = await _context.Payments.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync();
        }
        else if (prefix == "EX")
        {
            lastId = await _context.Expenses.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync();
        }

        return $"{prefix}-{DateTime.Now.ToString("yyMM")}-{(lastId + 1):D4}";
    }
}
