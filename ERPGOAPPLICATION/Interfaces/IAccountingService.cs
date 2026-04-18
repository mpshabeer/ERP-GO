using ERPGODomain.Entities;
using ERPGODomain.DTOs;

namespace ERPGOAPPLICATION.Interfaces;

public interface IAccountingService
{
    // Queries
    Task<List<AccountGroup>> GetAccountGroupsAsync();
    Task<List<AccountHead>> GetAccountHeadsAsync();
    Task<List<Account>> GetAccountsAsync();
    Task<Account?> GetAccountByIdAsync(int id);
    Task<Account?> GetAccountByNameAsync(string name);

    // Reports
    Task<LedgerReportResult> GetAccountLedgerAsync(LedgerReportRequest request);
    Task<TrialBalanceResult> GetTrialBalanceAsync(DateTime asOfDate);

    // Commands
    Task<Account> CreateAccountAsync(Account account);
    Task UpdateAccountAsync(Account account);
    Task DeleteAccountAsync(int id);

    // Journal Entries
    Task PostJournalEntryAsync(JournalEntry entry);
}
