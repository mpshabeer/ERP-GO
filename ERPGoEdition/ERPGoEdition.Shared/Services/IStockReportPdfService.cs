using ERPGODomain.DTOs;

namespace ERPGoEdition.Shared.Services;

public interface IStockReportPdfService
{
    Task<byte[]> GenerateStockReportPdfAsync(StockReportRequest request);
}
