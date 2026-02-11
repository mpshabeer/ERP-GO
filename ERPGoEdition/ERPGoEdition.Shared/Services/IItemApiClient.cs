
using ERPGODomain.Entities;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface IItemApiClient
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
