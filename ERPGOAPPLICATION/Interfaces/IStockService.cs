using ERPGODomain.Entities;
using ERPGODomain.DTOs;

namespace ERPGOAPPLICATION.Interfaces;

public interface IStockService
{
    Task AddStockTransaction(StockLedger entry); // Handles Ledger + Item.CurrentStock update
    Task AddStockTransactions(List<StockLedger> entries);
    Task SaveOpeningStock(ItemOpeningStock entry);
    Task<List<ItemOpeningStock>> GetItemOpeningStocks();
    Task<List<OpeningStockHistory>> GetOpeningStockHistories();
    
    // Context-specific methods
    Task<ItemOpeningStock?> GetItemOpeningStock(int itemId);
    Task<List<OpeningStockHistory>> GetOpeningStockHistory(int itemId);
    
    // Server-side Pagination
    Task<PaginatedResult<OpeningStockHistory>> GetPaginatedOpeningStockHistory(string? search, int page, int pageSize);

    Task<decimal> GetStock(int itemId);
}
