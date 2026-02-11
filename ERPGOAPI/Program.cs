using ERPGOAPPLICATION.Interfaces;
using ERPGOAPPLICATION.DTOs;
using ERPGOINFRASTRUCTURE.Services;
using ERPGOINFRASTRUCTURE.Services;
using ERPGOINFRASTRUCTURE;
using ERPGODomain.Entities;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Register Core Services (Infrastructure)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddInfrastructureServices(connectionString);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Minimal API Endpoint
app.MapPost("/api/auth/login", async ([FromBody] LoginRequest request, IAuthService authService) =>
{
    var result = await authService.LoginAsync(request);
    if (result.IsSuccess)
        return Results.Ok(result);
    return Results.BadRequest(result);
})
.WithName("Login");

// Unit Endpoints
app.MapGet("/api/units", async (IUnitService service) => await service.GetAllUnitsAsync());
app.MapGet("/api/units/{id}", async (int id, IUnitService service) => 
    await service.GetUnitByIdAsync(id) is Unit unit ? Results.Ok(unit) : Results.NotFound());
app.MapPost("/api/units", async ([FromBody] Unit unit, IUnitService service) => {
    var created = await service.AddUnitAsync(unit);
    return Results.Created($"/api/units/{created.Id}", created);
});
app.MapPut("/api/units", async ([FromBody] Unit unit, IUnitService service) => {
    await service.UpdateUnitAsync(unit);
    return Results.Ok(unit);
});
app.MapDelete("/api/units/{id}", async (int id, IUnitService service) => {
    await service.DeleteUnitAsync(id);
    return Results.NoContent();
});

// Item Endpoints
app.MapGet("/api/items", async (IItemService service) => await service.GetAllItemsAsync());
app.MapGet("/api/items/{id}", async (int id, IItemService service) => 
    await service.GetItemByIdAsync(id) is Item item ? Results.Ok(item) : Results.NotFound());
app.MapPost("/api/items", async ([FromBody] Item item, IItemService service) => {
    var created = await service.AddItemAsync(item);
    return Results.Created($"/api/items/{created.Id}", created);
});
app.MapPut("/api/items", async ([FromBody] Item item, IItemService service) => {
    await service.UpdateItemAsync(item);
    return Results.Ok(item);
});
app.MapDelete("/api/items/{id}", async (int id, IItemService service) => {
    await service.DeleteItemAsync(id);
    return Results.NoContent();
});

app.Run();

