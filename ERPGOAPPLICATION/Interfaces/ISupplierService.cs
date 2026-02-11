using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface ISupplierService
{
    Task<List<Supplier>> GetAllSuppliersAsync();
    Task<Supplier?> GetSupplierByIdAsync(int id);
    Task<Supplier> AddSupplierAsync(Supplier supplier);
    Task<Supplier> UpdateSupplierAsync(Supplier supplier);
    Task DeleteSupplierAsync(int id);
}
