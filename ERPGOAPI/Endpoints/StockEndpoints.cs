using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ERPGOAPI.Endpoints;

public static class StockEndpoints
{
    public static void MapStockEndpoints(this IEndpointRouteBuilder app)
    {
        var rootGroup = app.MapGroup("/api/stock");
        
        // Stock Ledger Operations
        rootGroup.MapPost("/transaction", async ([FromBody] StockLedger entry, IStockService service) => {
            // Replicating original logic
            await service.AddStockTransaction(entry);
            await service.AddStockTransaction(entry);
            return Results.Ok();
        });
        rootGroup.MapPost("/transactions", async ([FromBody] List<StockLedger> entries, IStockService service) => {
            await service.AddStockTransactions(entries);
            return Results.Ok();
        });
        rootGroup.MapGet("/{itemId}", async (int itemId, IStockService service) => {
            var stock = await service.GetStock(itemId);
            return Results.Ok(stock);
        });

        // Opening Stock Operations
        var openingGroup = rootGroup.MapGroup("/opening");
        openingGroup.MapPost("/", async ([FromBody] ItemOpeningStock entry, IStockService service) => {
            try 
            {
                await service.SaveOpeningStock(entry);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        openingGroup.MapGet("/current", async (IStockService service) => {
            var result = await service.GetItemOpeningStocks();
            return Results.Ok(result);
        });

        openingGroup.MapGet("/history", async (IStockService service) => {
            var result = await service.GetOpeningStockHistories();
            return Results.Ok(result);
        });

        openingGroup.MapGet("/{itemId}", async (int itemId, IStockService service) => {
            var result = await service.GetItemOpeningStock(itemId);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        openingGroup.MapGet("/history/{itemId}", async (int itemId, IStockService service) => {
            var result = await service.GetOpeningStockHistory(itemId);
            return Results.Ok(result);
        });

        openingGroup.MapGet("/history/search", async (string? search, int page, int pageSize, IStockService service) => {
            var result = await service.GetPaginatedOpeningStockHistory(search, page, pageSize);
            return Results.Ok(result);
        });

        // Stock Adjustment Operations
        var adjustmentGroup = rootGroup.MapGroup("/adjustment");
        adjustmentGroup.MapPost("/", async ([FromBody] StockAdjustmentHeader header, IStockAdjustmentService service) =>
        {
            return await service.CreateAdjustment(header);
        })
        .WithName("CreateStockAdjustment")
        .WithOpenApi();

        adjustmentGroup.MapGet("/", async (IStockAdjustmentService service) =>
        {
            return await service.GetAdjustments();
        })
        .WithName("GetStockAdjustments")
        .WithOpenApi();

        adjustmentGroup.MapGet("/{id}", async (int id, IStockAdjustmentService service) =>
        {
            var result = await service.GetAdjustment(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetStockAdjustment")
        .WithOpenApi();
    }
}
