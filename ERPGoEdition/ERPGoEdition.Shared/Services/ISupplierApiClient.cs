using ERPGODomain.Entities;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface ISupplierApiClient
{
    [Get("/api/suppliers")]
    Task<List<Supplier>> GetAllSuppliersAsync();

    [Get("/api/suppliers/{id}")]
    Task<Supplier?> GetSupplierByIdAsync(int id);

    [Post("/api/suppliers")]
    Task<Supplier> AddSupplierAsync([Body] Supplier supplier);

    [Put("/api/suppliers")]
    Task<Supplier> UpdateSupplierAsync([Body] Supplier supplier);

    [Delete("/api/suppliers/{id}")]
    Task DeleteSupplierAsync(int id);
}
