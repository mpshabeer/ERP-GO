using ERPGODomain.DTOs;
using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface IPurchaseService
{
    Task<Purchase> CreatePurchase(Purchase purchase);
    Task<Purchase> UpdatePurchase(Purchase purchase);
    Task<string> GetNextPurchaseNumber();
    Task<PagedResult<Purchase>> GetPurchasesAsync(PurchaseSearchRequest request);
    Task<Purchase?> GetPurchaseByIdAsync(int id);
    Task DeletePurchaseAsync(int id);
}
