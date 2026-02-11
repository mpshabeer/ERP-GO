using ERPGODomain.Entities;
using Refit;

namespace ERPGOAPPLICATION.Interfaces;

public interface IItemApi
{
    [Get("/api/items")]
    Task<List<Item>> GetAllItemsAsync();

    [Get("/api/items/{id}")]
    Task<Item?> GetItemByIdAsync(int id);

    [Post("/api/items")]
    Task<Item> AddItemAsync([Body] Item item);

    [Put("/api/items")]
    Task<Item> UpdateItemAsync([Body] Item item);

    [Delete("/api/items/{id}")]
    Task DeleteItemAsync(int id);
}
