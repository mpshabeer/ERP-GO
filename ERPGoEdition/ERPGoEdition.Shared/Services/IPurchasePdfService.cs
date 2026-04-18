namespace ERPGoEdition.Shared.Services;

public interface IPurchasePdfService
{
    Task<byte[]> GeneratePurchaseInvoicePdfAsync(int purchaseId);
}
