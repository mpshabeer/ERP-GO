using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGODomain.DTOs;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class AccountingService : IAccountingService
{
    private readonly ApplicationDbContext _context;

    public AccountingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountGroup>> GetAccountGroupsAsync()
    {
        return await _context.AccountGroups.AsNoTracking().ToListAsync();
    }

    public async Task<List<AccountHead>> GetAccountHeadsAsync()
    {
        return await _context.AccountHeads
            .Include(h => h.AccountGroup)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Account>> GetAccountsAsync()
    {
        return await _context.Accounts
            .Include(a => a.AccountHead)
            .ThenInclude(h => h.AccountGroup)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        return await _context.Accounts
            .Include(a => a.AccountHead)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Account?> GetAccountByNameAsync(string name)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Name == name);
    }

    public async Task<Account> CreateAccountAsync(Account account)
    {
        if (await _context.Accounts.AnyAsync(a => a.Name == account.Name))
        {
            throw new Exception($"Account '{account.Name}' already exists.");
        }

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task UpdateAccountAsync(Account account)
    {
        var existing = await _context.Accounts.FindAsync(account.Id);
        if (existing == null) throw new Exception("Account not found");

        if (existing.IsDefault)
        {
            throw new Exception("Default accounts cannot be modified manually through this module.");
        }

        _context.Entry(existing).CurrentValues.SetValues(account);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null) return;

        if (account.IsDefault)
        {
            throw new Exception("Default accounts cannot be deleted.");
        }

        bool hasTransactions = await _context.JournalEntryLines.AnyAsync(l => l.AccountId == id);
        if (hasTransactions)
        {
            throw new Exception("Cannot delete account because it has associated transactions.");
        }

        bool hasCustomers = await _context.Customers.AnyAsync(c => c.AccountId == id);
        bool hasSuppliers = await _context.Suppliers.AnyAsync(s => s.AccountId == id);
        if (hasCustomers || hasSuppliers)
        {
             throw new Exception("Cannot delete account because it is linked to customers or suppliers.");
        }

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
    }

    public async Task PostJournalEntryAsync(JournalEntry entry)
    {
        if (entry.JournalEntryLines == null || entry.JournalEntryLines.Count < 2)
        {
            throw new Exception("Journal entry must have at least two lines.");
        }

        decimal totalDr = entry.JournalEntryLines.Sum(l => l.Debit);
        decimal totalCr = entry.JournalEntryLines.Sum(l => l.Credit);

        if (totalDr != totalCr)
        {
            throw new Exception($"Journal entry must balance. Dr: {totalDr}, Cr: {totalCr}");
        }

        _context.JournalEntries.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<LedgerReportResult> GetAccountLedgerAsync(LedgerReportRequest request)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId);
        if (account == null) throw new Exception("Account not found");

        var query = _context.JournalEntryLines
            .Include(l => l.JournalEntry)
            .Where(l => l.AccountId == request.AccountId)
            .AsQueryable();

        decimal openingBalance = 0m;

        if (request.FromDate.HasValue)
        {
            var priorEntries = await query
                .Where(l => l.JournalEntry.VoucherDate < request.FromDate.Value)
                .ToListAsync();
            openingBalance = priorEntries.Sum(l => l.Debit - l.Credit);
            
            query = query.Where(l => l.JournalEntry.VoucherDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            // Set time to end of day if it's just a date
            var toDateEndOfDay = request.ToDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(l => l.JournalEntry.VoucherDate <= toDateEndOfDay);
        }

        var lines = await query
            .OrderBy(l => l.JournalEntry.VoucherDate)
            .ThenBy(l => l.Id)
            .ToListAsync();

        var result = new LedgerReportResult
        {
            AccountName = account.Name,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            OpeningBalance = openingBalance
        };

        decimal currentBalance = openingBalance;
        foreach (var line in lines)
        {
            currentBalance += (line.Debit - line.Credit);
            result.Lines.Add(new LedgerReportLine
            {
                Date = line.JournalEntry.VoucherDate,
                VoucherNo = line.JournalEntry.VoucherNo,
                Description = line.Description,
                Debit = line.Debit,
                Credit = line.Credit,
                Balance = currentBalance
            });
        }

        result.TotalDebit = result.Lines.Sum(l => l.Debit);
        result.TotalCredit = result.Lines.Sum(l => l.Credit);
        result.ClosingBalance = currentBalance;

        return result;
    }

    public async Task<TrialBalanceResult> GetTrialBalanceAsync(DateTime asOfDate)
    {
        var asOfDateEndOfDay = asOfDate.Date.AddDays(1).AddTicks(-1);
        
        var accounts = await _context.Accounts
            .Include(a => a.AccountHead)
            .ThenInclude(h => h.AccountGroup)
            .ToListAsync();

        var entryLines = await _context.JournalEntryLines
            .Include(l => l.JournalEntry)
            .Where(l => l.JournalEntry.VoucherDate <= asOfDateEndOfDay)
            .ToListAsync();

        var result = new TrialBalanceResult { AsOfDate = asOfDate };

        foreach (var account in accounts.OrderBy(a => a.AccountHead.AccountGroup.Id).ThenBy(a => a.AccountHead.Id).ThenBy(a => a.Name))
        {
            var accLines = entryLines.Where(l => l.AccountId == account.Id).ToList();
            if (!accLines.Any()) continue;

            decimal totalDr = accLines.Sum(l => l.Debit);
            decimal totalCr = accLines.Sum(l => l.Credit);
            decimal netBalance = totalDr - totalCr;

            if (netBalance != 0)
            {
                result.Lines.Add(new TrialBalanceLine
                {
                    AccountGroupName = account.AccountHead.AccountGroup.Name,
                    AccountHeadName = account.AccountHead.Name,
                    AccountName = account.Name,
                    DebitBalance = netBalance > 0 ? netBalance : 0,
                    CreditBalance = netBalance < 0 ? Math.Abs(netBalance) : 0
                });
            }
        }

        result.TotalDebit = result.Lines.Sum(l => l.DebitBalance);
        result.TotalCredit = result.Lines.Sum(l => l.CreditBalance);

        return result;
    }
}
