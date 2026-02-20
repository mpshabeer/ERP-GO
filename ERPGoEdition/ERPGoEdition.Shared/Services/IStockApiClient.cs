using ERPGODomain.Entities;
using Refit;
using ERPGOAPPLICATION;
using ERPGODomain.DTOs;

namespace ERPGoEdition.Shared.Services;

public interface IStockApiClient
{
    [Post("/api/stock/transaction")]
    Task AddStockTransaction([Body] StockLedger entry);

    [Post("/api/stock/transactions")]
    Task AddStockTransactions([Body] List<StockLedger> entries);

    [Get("/api/stock/{itemId}")]
    Task<decimal> GetStock(int itemId);

    [Post("/api/stock/opening")]
    Task SaveOpeningStock([Body] ItemOpeningStock entry);

    [Get("/api/stock/opening/current")]
    Task<List<ItemOpeningStock>> GetItemOpeningStocks();

    [Get("/api/stock/opening/history")]
    Task<List<OpeningStockHistory>> GetOpeningStockHistories();

    [Get("/api/stock/opening/{itemId}")]
    Task<ItemOpeningStock?> GetItemOpeningStock(int itemId);

    [Get("/api/stock/opening/history/{itemId}")]
    Task<List<OpeningStockHistory>> GetOpeningStockHistory(int itemId);

    [Get("/api/stock/opening/history/search")]
    Task<PaginatedResult<OpeningStockHistory>> GetPaginatedOpeningStockHistory(string? search, int page, int pageSize);
}
