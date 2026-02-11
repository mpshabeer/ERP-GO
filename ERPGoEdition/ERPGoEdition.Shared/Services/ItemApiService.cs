
using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;

namespace ERPGoEdition.Shared.Services;

public class ItemApiService : IItemService
{
    private readonly IItemApiClient _apiClient;

    public ItemApiService(IItemApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<List<Item>> GetAllItemsAsync() => _apiClient.GetAllItemsAsync();
    public Task<Item?> GetItemByIdAsync(int id) => _apiClient.GetItemByIdAsync(id);
    public Task<Item> AddItemAsync(Item item) => _apiClient.AddItemAsync(item);
    public Task<Item> UpdateItemAsync(Item item) => _apiClient.UpdateItemAsync(item);
    public Task DeleteItemAsync(int id) => _apiClient.DeleteItemAsync(id);
}
