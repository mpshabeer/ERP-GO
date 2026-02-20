using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;

namespace ERPGoEdition.Shared.Services;

public class StockAdjustmentApiService : IStockAdjustmentService
{
    private readonly IStockAdjustmentApiClient _apiClient;

    public StockAdjustmentApiService(IStockAdjustmentApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<string> CreateAdjustment(StockAdjustmentHeader header)
    {
        return await _apiClient.CreateAdjustment(header);
    }

    public async Task<List<StockAdjustmentHeader>> GetAdjustments()
    {
        return await _apiClient.GetAdjustments();
    }

    public async Task<StockAdjustmentHeader?> GetAdjustment(int id)
    {
        return await _apiClient.GetAdjustment(id);
    }
}
