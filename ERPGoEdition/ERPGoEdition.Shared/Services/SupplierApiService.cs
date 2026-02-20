using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;

namespace ERPGoEdition.Shared.Services;

public class SupplierApiService : ISupplierService
{
    private readonly ISupplierApiClient _apiClient;

    public SupplierApiService(ISupplierApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        return await _apiClient.GetAllSuppliersAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int id)
    {
        try
        {
            return await _apiClient.GetSupplierByIdAsync(id);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Supplier> AddSupplierAsync(Supplier supplier)
    {
        return await _apiClient.AddSupplierAsync(supplier);
    }

    public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
    {
        return await _apiClient.UpdateSupplierAsync(supplier);
    }

    public async Task DeleteSupplierAsync(int id)
    {
        await _apiClient.DeleteSupplierAsync(id);
    }
}
