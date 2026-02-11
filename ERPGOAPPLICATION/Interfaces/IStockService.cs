using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface IStockService
{
    Task AddStockTransaction(StockLedger entry); // Handles Ledger + Item.CurrentStock update
    Task<decimal> GetStock(int itemId);
}
