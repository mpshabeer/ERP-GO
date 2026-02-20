using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;

namespace ERPGoEdition.Shared.Services;

public class CustomerApiService : ICustomerService
{
    private readonly ICustomerApiClient _apiClient;

    public CustomerApiService(ICustomerApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        return await _apiClient.GetAllCustomersAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        try
        {
            return await _apiClient.GetCustomerByIdAsync(id);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        return await _apiClient.AddCustomerAsync(customer);
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        return await _apiClient.UpdateCustomerAsync(customer);
    }

    public async Task DeleteCustomerAsync(int id)
    {
        await _apiClient.DeleteCustomerAsync(id);
    }
}
