using ERPGODomain.DTOs;

namespace ERPGoEdition.Shared.Services;

public interface IReportPdfService
{
    Task<byte[]> GenerateSalesReportAsync(SalesReportRequest request, List<ItemWiseReportLine> lines);
    Task<byte[]> GeneratePurchaseReportAsync(PurchaseReportRequest request, List<ItemWiseReportLine> lines);
    
    Task<byte[]> GenerateAccountLedgerPdfAsync(LedgerReportResult result);
    Task<byte[]> GenerateTrialBalancePdfAsync(TrialBalanceResult result);
}
