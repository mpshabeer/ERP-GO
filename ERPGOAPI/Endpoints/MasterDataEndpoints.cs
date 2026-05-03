using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using Microsoft.AspNetCore.Mvc;
using Unit = ERPGODomain.Entities.Unit;

namespace ERPGOAPI.Endpoints;

public static class MasterDataEndpoints
{
    public static void MapMasterDataEndpoints(this IEndpointRouteBuilder app)
    {
        MapUnitEndpoints(app);
        MapItemEndpoints(app);
        MapCategoryEndpoints(app);
    }

    private static void MapUnitEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/units");

        group.MapGet("/", async (IUnitService service) => await service.GetAllUnitsAsync());
        group.MapGet("/all", async (IUnitService service) => await service.GetAllUnitsIncludeInactiveAsync());
        group.MapGet("/{id}", async (int id, IUnitService service) => 
            await service.GetUnitByIdAsync(id) is Unit unit ? Results.Ok(unit) : Results.NotFound());
        group.MapPost("/", async ([FromBody] Unit unit, IUnitService service) => {
            var created = await service.AddUnitAsync(unit);
            return Results.Created($"/api/units/{created.Id}", created);
        });
        group.MapPut("/", async ([FromBody] Unit unit, IUnitService service) => {
            await service.UpdateUnitAsync(unit);
            return Results.Ok(unit);
        });
        group.MapDelete("/{id}", async (int id, IUnitService service) => {
            await service.DeleteUnitAsync(id);
            return Results.NoContent();
        });
    }

    private static void MapItemEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/items");

        group.MapGet("/", async (IItemService service) => await service.GetAllItemsAsync());
        group.MapGet("/{id}", async (int id, IItemService service) => 
            await service.GetItemByIdAsync(id) is Item item ? Results.Ok(item) : Results.NotFound());
        group.MapPost("/", async ([FromBody] Item item, IItemService service) => {
            var created = await service.AddItemAsync(item);
            return Results.Created($"/api/items/{created.Id}", created);
        });
        group.MapPut("/", async ([FromBody] Item item, IItemService service) => {
            await service.UpdateItemAsync(item);
            return Results.Ok(item);
        });
        group.MapDelete("/{id}", async (int id, IItemService service) => {
            await service.DeleteItemAsync(id);
            return Results.NoContent();
        });
        group.MapGet("/next-code", async (string prefix, int startNumber, IItemService service) => {
            var code = await service.GetNextItemCodeAsync(prefix, startNumber);
            return Results.Ok(code);
        });
    }

    private static void MapCategoryEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories");

        group.MapGet("/", async (ICategoryService service) => await service.GetAllCategoriesAsync());
        group.MapGet("/{id}", async (int id, ICategoryService service) => 
            await service.GetCategoryByIdAsync(id) is Category category ? Results.Ok(category) : Results.NotFound());
        group.MapPost("/", async ([FromBody] Category category, ICategoryService service) => {
            var created = await service.AddCategoryAsync(category);
            return Results.Created($"/api/categories/{created.Id}", created);
        });
        group.MapPut("/", async ([FromBody] Category category, ICategoryService service) => {
            await service.UpdateCategoryAsync(category);
            return Results.Ok(category);
        });
        group.MapDelete("/{id}", async (int id, ICategoryService service) => {
            await service.DeleteCategoryAsync(id);
            return Results.NoContent();
        });
    }
}
