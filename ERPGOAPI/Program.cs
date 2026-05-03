using ERPGOAPPLICATION.DTOs;
using ERPGOAPPLICATION.Interfaces;
using ERPGOAPI.Reports;
using ERPGODomain.Entities;
using ERPGOINFRASTRUCTURE;
using ERPGOINFRASTRUCTURE.Services;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Unit = ERPGODomain.Entities.Unit;
using ERPGOAPI.Endpoints;

// Configure QuestPDF community license (free for revenue < $1M)
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

// Configure Minimal APIs JSON serialization to ignore object cycles
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Register all endpoints from the Endpoints framework folder
app.MapAllEndpoints();

app.Run();

