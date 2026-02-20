using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOAPPLICATION;
using ERPGODomain.DTOs;

namespace ERPGoEdition.Shared.Services;

public class StockApiService : IStockService
{
    private readonly IStockApiClient _apiClient;

    public StockApiService(IStockApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task AddStockTransaction(StockLedger entry)
    {
        await _apiClient.AddStockTransaction(entry);
    }

    public async Task AddStockTransactions(List<StockLedger> entries)
    {
        await _apiClient.AddStockTransactions(entries);
    }

    public async Task<decimal> GetStock(int itemId)
    {
        return await _apiClient.GetStock(itemId);
    }

    public async Task SaveOpeningStock(ItemOpeningStock entry)
    {
        await _apiClient.SaveOpeningStock(entry);
    }

    public async Task<List<ItemOpeningStock>> GetItemOpeningStocks()
    {
        return await _apiClient.GetItemOpeningStocks();
    }

    public async Task<List<OpeningStockHistory>> GetOpeningStockHistories()
    {
        return await _apiClient.GetOpeningStockHistories();
    }

    public async Task<ItemOpeningStock?> GetItemOpeningStock(int itemId)
    {
        return await _apiClient.GetItemOpeningStock(itemId);
    }

    public async Task<List<OpeningStockHistory>> GetOpeningStockHistory(int itemId)
    {
        return await _apiClient.GetOpeningStockHistory(itemId);
    }

    public async Task<PaginatedResult<OpeningStockHistory>> GetPaginatedOpeningStockHistory(string? search, int page, int pageSize)
    {
        return await _apiClient.GetPaginatedOpeningStockHistory(search, page, pageSize);
    }
}
