using ERPGODomain.Entities;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface ICustomerApiClient
{
    [Get("/api/customers")]
    Task<List<Customer>> GetAllCustomersAsync();

    [Get("/api/customers/{id}")]
    Task<Customer?> GetCustomerByIdAsync(int id);

    [Post("/api/customers")]
    Task<Customer> AddCustomerAsync([Body] Customer customer);

    [Put("/api/customers")]
    Task<Customer> UpdateCustomerAsync([Body] Customer customer);

    [Delete("/api/customers/{id}")]
    Task DeleteCustomerAsync(int id);
}
