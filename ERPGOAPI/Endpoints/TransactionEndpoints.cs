using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ERPGOAPI.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        MapSalesEndpoints(app);
        MapPurchaseEndpoints(app);
        MapGstSalesEndpoints(app);
    }

    private static void MapSalesEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sales/invoice");

        group.MapPost("/", async ([FromBody] SalesInvoice invoice, ISalesService service) => {
            try 
            {
                var created = await service.CreateInvoice(invoice);
                return Results.Created($"/api/sales/invoice/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        group.MapGet("/next-number", async (ISalesService service) => {
            var number = await service.GetNextInvoiceNumber();
            return Results.Ok(number);
        });

        app.MapGet("/api/sales/invoices", async (
            [FromQuery] string? invoiceNo,
            [FromQuery] string? customerName,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            ISalesService service) =>
        {
            var request = new ERPGODomain.DTOs.InvoiceSearchRequest
            {
                InvoiceNo = invoiceNo,
                CustomerName = customerName,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 ? 15 : pageSize
            };
            var result = await service.GetInvoicesAsync(request);
            return Results.Ok(result);
        });

        group.MapPut("/", async ([FromBody] SalesInvoice invoice, ISalesService service) =>
        {
            try
            {
                var result = await service.UpdateInvoice(invoice);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }

    private static void MapPurchaseEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/purchases");

        group.MapPost("/", async ([FromBody] Purchase purchase, IPurchaseService service) => {
            try 
            {
                var created = await service.CreatePurchase(purchase);
                return Results.Created($"/api/purchases/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        group.MapGet("/next-number", async (IPurchaseService service) => {
            var number = await service.GetNextPurchaseNumber();
            return Results.Ok(number);
        });

        group.MapGet("/", async (
            [FromQuery] string? purchaseNo,
            [FromQuery] string? invoiceNo,
            [FromQuery] string? supplierName,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IPurchaseService service) =>
        {
            var request = new ERPGODomain.DTOs.PurchaseSearchRequest
            {
                PurchaseNo = purchaseNo,
                InvoiceNo = invoiceNo,
                SupplierName = supplierName,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Page = page < 1 ? 1 : page,
                PageSize = pageSize < 1 ? 15 : pageSize
            };
            var result = await service.GetPurchasesAsync(request);
            return Results.Ok(result);
        });

        group.MapGet("/{id}", async (int id, IPurchaseService service) => 
            await service.GetPurchaseByIdAsync(id) is Purchase purchase ? Results.Ok(purchase) : Results.NotFound());

        group.MapPut("/", async ([FromBody] Purchase purchase, IPurchaseService service) =>
        {
            try
            {
                var result = await service.UpdatePurchase(purchase);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        group.MapDelete("/{id}", async (int id, IPurchaseService service) => {
            try
            {
                await service.DeletePurchaseAsync(id);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }

    private static void MapGstSalesEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/gstsales/invoice");

        group.MapPost("/", async ([FromBody] GstSalesInvoice invoice, IGstSalesInvoiceService service) => {
            try
            {
                var created = await service.CreateInvoice(invoice);
                return Results.Created($"/api/gstsales/invoice/{created.Id}", created);
            }
            catch (Exception ex) { return Results.BadRequest(ex.Message); }
        });

        group.MapGet("/next-number", async (IGstSalesInvoiceService service) => {
            var number = await service.GetNextInvoiceNumber();
            return Results.Ok(number);
        });

        app.MapGet("/api/gstsales/invoices", async (IGstSalesInvoiceService service) => {
            var list = await service.GetInvoicesAsync();
            return Results.Ok(list);
        });

        group.MapGet("/{id}", async (int id, IGstSalesInvoiceService service) => {
            var invoice = await service.GetInvoiceByIdAsync(id);
            return invoice is null ? Results.NotFound() : Results.Ok(invoice);
        });

        group.MapPut("/", async ([FromBody] GstSalesInvoice invoice, IGstSalesInvoiceService service) => {
            try
            {
                var result = await service.UpdateInvoice(invoice);
                return Results.Ok(result);
            }
            catch (Exception ex) { return Results.BadRequest(ex.Message); }
        });

        group.MapDelete("/{id}", async (int id, IGstSalesInvoiceService service) => {
            try
            {
                await service.DeleteInvoiceAsync(id);
                return Results.NoContent();
            }
            catch (Exception ex) { return Results.BadRequest(ex.Message); }
        });
    }
}
