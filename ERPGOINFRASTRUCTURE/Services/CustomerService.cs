using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPGOINFRASTRUCTURE.Services;

public class CustomerService : ICustomerService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CustomerService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Customers.ToListAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var existingCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
        if (existingCustomer != null)
        {
            context.Entry(existingCustomer).CurrentValues.SetValues(customer);
            await context.SaveChangesAsync();
            return existingCustomer;
        }
        return customer;
    }

    public async Task DeleteCustomerAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer != null)
        {
            customer.IsActive = false;
            await context.SaveChangesAsync();
        }
    }
}
