using ERPGOAPPLICATION.DTOs;
using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE;
using ERPGOINFRASTRUCTURE.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

// Register Core Services (Infrastructure)
builder.Services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();
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
app.MapGet("/api/items/next-code", async (string prefix, int startNumber, IItemService service) => {
    var code = await service.GetNextItemCodeAsync(prefix, startNumber);
    return Results.Ok(code);
});

// Customer Endpoints
app.MapGet("/api/customers", async (ICustomerService service) => await service.GetAllCustomersAsync());
app.MapGet("/api/customers/{id}", async (int id, ICustomerService service) => 
    await service.GetCustomerByIdAsync(id) is Customer customer ? Results.Ok(customer) : Results.NotFound());
app.MapPost("/api/customers", async ([FromBody] Customer customer, ICustomerService service) => {
    var created = await service.AddCustomerAsync(customer);
    return Results.Created($"/api/customers/{created.Id}", created);
});
app.MapPut("/api/customers", async ([FromBody] Customer customer, ICustomerService service) => {
    await service.UpdateCustomerAsync(customer);
    return Results.Ok(customer);
});
app.MapDelete("/api/customers/{id}", async (int id, ICustomerService service) => {
    await service.DeleteCustomerAsync(id);
    return Results.NoContent();
});

// Supplier Endpoints
app.MapGet("/api/suppliers", async (ISupplierService service) => await service.GetAllSuppliersAsync());
app.MapGet("/api/suppliers/{id}", async (int id, ISupplierService service) => 
    await service.GetSupplierByIdAsync(id) is Supplier supplier ? Results.Ok(supplier) : Results.NotFound());
app.MapPost("/api/suppliers", async ([FromBody] Supplier supplier, ISupplierService service) => {
    var created = await service.AddSupplierAsync(supplier);
    return Results.Created($"/api/suppliers/{created.Id}", created);
});
app.MapPut("/api/suppliers", async ([FromBody] Supplier supplier, ISupplierService service) => {
    await service.UpdateSupplierAsync(supplier);
    return Results.Ok(supplier);
});
app.MapDelete("/api/suppliers/{id}", async (int id, ISupplierService service) => {
    await service.DeleteSupplierAsync(id);
    return Results.NoContent();
});

// Stock Endpoints
app.MapPost("/api/stock/transaction", async ([FromBody] StockLedger entry, IStockService service) => {
    await service.AddStockTransaction(entry);
    await service.AddStockTransaction(entry);
    return Results.Ok();
});
app.MapPost("/api/stock/transactions", async ([FromBody] List<StockLedger> entries, IStockService service) => {
    await service.AddStockTransactions(entries);
    return Results.Ok();
});
app.MapGet("/api/stock/{itemId}", async (int itemId, IStockService service) => {
    var stock = await service.GetStock(itemId);
    return Results.Ok(stock);
});

// Opening Stock Endpoints
app.MapPost("/api/stock/opening", async ([FromBody] ItemOpeningStock entry, IStockService service) => {
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

app.MapGet("/api/stock/opening/current", async (IStockService service) => {
    var result = await service.GetItemOpeningStocks();
    return Results.Ok(result);
});

app.MapGet("/api/stock/opening/history", async (IStockService service) => {
    var result = await service.GetOpeningStockHistories();
    return Results.Ok(result);
});

app.MapGet("/api/stock/opening/{itemId}", async (int itemId, IStockService service) => {
    var result = await service.GetItemOpeningStock(itemId);
    return result is not null ? Results.Ok(result) : Results.NotFound();
});

app.MapGet("/api/stock/opening/history/{itemId}", async (int itemId, IStockService service) => {
    var result = await service.GetOpeningStockHistory(itemId);
    return Results.Ok(result);
});

app.MapGet("/api/stock/opening/history/search", async (string? search, int page, int pageSize, IStockService service) => {
    // Default page=0, pageSize=10 if not provided (handled by client or defaults here)
    var result = await service.GetPaginatedOpeningStockHistory(search, page, pageSize);
    return Results.Ok(result);
});

// Stock Adjustment Endpoints
app.MapPost("/api/stock/adjustment", async ([FromBody] StockAdjustmentHeader header, IStockAdjustmentService service) =>
{
    return await service.CreateAdjustment(header);
})
.WithName("CreateStockAdjustment")
.WithOpenApi();

app.MapGet("/api/stock/adjustment", async (IStockAdjustmentService service) =>
{
    return await service.GetAdjustments();
})
.WithName("GetStockAdjustments")
.WithOpenApi();

app.MapGet("/api/stock/adjustment/{id}", async (int id, IStockAdjustmentService service) =>
{
    // Need to handle null
    var result = await service.GetAdjustment(id);
    return result is null ? Results.NotFound() : Results.Ok(result);
})
.WithName("GetStockAdjustment")
.WithOpenApi();

// Sales Endpoints
app.MapPost("/api/sales/invoice", async ([FromBody] SalesInvoice invoice, ISalesService service) => {
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

app.MapGet("/api/sales/invoice/next-number", async (ISalesService service) => {
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

app.MapPut("/api/sales/invoice", async ([FromBody] ERPGODomain.Entities.SalesInvoice invoice, ISalesService service) =>
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

// Purchase Endpoints
// Purchase Endpoints
app.MapPost("/api/purchases", async ([FromBody] Purchase purchase, IPurchaseService service) => {
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

app.MapGet("/api/purchases/next-number", async (IPurchaseService service) => {
    var number = await service.GetNextPurchaseNumber();
    return Results.Ok(number);
});

app.MapGet("/api/purchases", async (
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

app.MapGet("/api/purchases/{id}", async (int id, IPurchaseService service) => 
    await service.GetPurchaseByIdAsync(id) is Purchase purchase ? Results.Ok(purchase) : Results.NotFound());

app.MapPut("/api/purchases", async ([FromBody] Purchase purchase, IPurchaseService service) =>
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

app.MapDelete("/api/purchases/{id}", async (int id, IPurchaseService service) => {
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


// Category Endpoints
app.MapGet("/api/categories", async (ICategoryService service) => await service.GetAllCategoriesAsync());
app.MapGet("/api/categories/{id}", async (int id, ICategoryService service) => 
    await service.GetCategoryByIdAsync(id) is Category category ? Results.Ok(category) : Results.NotFound());
app.MapPost("/api/categories", async ([FromBody] Category category, ICategoryService service) => {
    var created = await service.AddCategoryAsync(category);
    return Results.Created($"/api/categories/{created.Id}", created);
});
app.MapPut("/api/categories", async ([FromBody] Category category, ICategoryService service) => {
    await service.UpdateCategoryAsync(category);
    return Results.Ok(category);
});
app.MapDelete("/api/categories/{id}", async (int id, ICategoryService service) => {
    await service.DeleteCategoryAsync(id);
    return Results.NoContent();
});

app.Run();

