using ERPGODomain.DTOs;
using ERPGODomain.Entities;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface IPurchaseApiClient
{
    [Get("/api/purchases")]
    Task<PagedResult<Purchase>> GetPurchasesAsync(
        [AliasAs("purchaseNo")] string? purchaseNo,
        [AliasAs("invoiceNo")] string? invoiceNo,
        [AliasAs("supplierName")] string? supplierName,
        [AliasAs("dateFrom")] DateTime? dateFrom,
        [AliasAs("dateTo")] DateTime? dateTo,
        [AliasAs("page")] int page,
        [AliasAs("pageSize")] int pageSize);

    [Get("/api/purchases/{id}")]
    Task<Purchase> GetPurchaseByIdAsync(int id);

    [Get("/api/purchases/next-number")]
    Task<string> GetNextPurchaseNumber();

    [Post("/api/purchases")]
    Task<Purchase> CreatePurchase([Body] Purchase purchase);

    [Put("/api/purchases")]
    Task<Purchase> UpdatePurchase([Body] Purchase purchase);

    [Delete("/api/purchases/{id}")]
    Task DeletePurchaseAsync(int id);
}
