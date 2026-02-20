using ERPGODomain.Entities;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface IStockAdjustmentApiClient
{
    [Post("/api/stock/adjustment")]
    Task<string> CreateAdjustment([Body] StockAdjustmentHeader header);

    [Get("/api/stock/adjustment")]
    Task<List<StockAdjustmentHeader>> GetAdjustments();

    [Get("/api/stock/adjustment/{id}")]
    Task<StockAdjustmentHeader?> GetAdjustment(int id);
}
