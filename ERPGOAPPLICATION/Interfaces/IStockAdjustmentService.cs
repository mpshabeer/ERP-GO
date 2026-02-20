using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface IStockAdjustmentService
{
    Task<string> CreateAdjustment(StockAdjustmentHeader header); // Returns AdjustmentNo
    Task<List<StockAdjustmentHeader>> GetAdjustments();
    Task<StockAdjustmentHeader?> GetAdjustment(int id);
}
