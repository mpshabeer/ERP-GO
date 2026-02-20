using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.DTOs;
using ERPGODomain.Entities;

namespace ERPGoEdition.Shared.Services;

public class PurchaseApiService : IPurchaseService
{
    private readonly IPurchaseApiClient _apiClient;

    public PurchaseApiService(IPurchaseApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<string> GetNextPurchaseNumber()
    {
        return await _apiClient.GetNextPurchaseNumber();
    }

    public async Task<PagedResult<Purchase>> GetPurchasesAsync(PurchaseSearchRequest request)
    {
        return await _apiClient.GetPurchasesAsync(
            request.PurchaseNo,
            request.InvoiceNo,
            request.SupplierName,
            request.DateFrom,
            request.DateTo,
            request.Page,
            request.PageSize);
    }

    public async Task<Purchase?> GetPurchaseByIdAsync(int id)
    {
        try
        {
            return await _apiClient.GetPurchaseByIdAsync(id);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Purchase> CreatePurchase(Purchase purchase)
    {
        return await _apiClient.CreatePurchase(purchase);
    }

    public async Task<Purchase> UpdatePurchase(Purchase purchase)
    {
        return await _apiClient.UpdatePurchase(purchase);
    }

    public async Task DeletePurchaseAsync(int id)
    {
        await _apiClient.DeletePurchaseAsync(id);
    }
}
