using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ERPGOAPI.Endpoints;

public static class PartyEndpoints
{
    public static void MapPartyEndpoints(this IEndpointRouteBuilder app)
    {
        MapCustomerEndpoints(app);
        MapSupplierEndpoints(app);
    }

    private static void MapCustomerEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers");

        group.MapGet("/", async (ICustomerService service) => await service.GetAllCustomersAsync());
        group.MapGet("/{id}", async (int id, ICustomerService service) => 
            await service.GetCustomerByIdAsync(id) is Customer customer ? Results.Ok(customer) : Results.NotFound());
        group.MapPost("/", async ([FromBody] Customer customer, ICustomerService service) => {
            var created = await service.AddCustomerAsync(customer);
            return Results.Created($"/api/customers/{created.Id}", created);
        });
        group.MapPut("/", async ([FromBody] Customer customer, ICustomerService service) => {
            await service.UpdateCustomerAsync(customer);
            return Results.Ok(customer);
        });
        group.MapDelete("/{id}", async (int id, ICustomerService service) => {
            await service.DeleteCustomerAsync(id);
            return Results.NoContent();
        });
    }

    private static void MapSupplierEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suppliers");

        group.MapGet("/", async (ISupplierService service) => await service.GetAllSuppliersAsync());
        group.MapGet("/{id}", async (int id, ISupplierService service) => 
            await service.GetSupplierByIdAsync(id) is Supplier supplier ? Results.Ok(supplier) : Results.NotFound());
        group.MapPost("/", async ([FromBody] Supplier supplier, ISupplierService service) => {
            var created = await service.AddSupplierAsync(supplier);
            return Results.Created($"/api/suppliers/{created.Id}", created);
        });
        group.MapPut("/", async ([FromBody] Supplier supplier, ISupplierService service) => {
            await service.UpdateSupplierAsync(supplier);
            return Results.Ok(supplier);
        });
        group.MapDelete("/{id}", async (int id, ISupplierService service) => {
            await service.DeleteSupplierAsync(id);
            return Results.NoContent();
        });
    }
}
