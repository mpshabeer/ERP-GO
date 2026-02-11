
using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface IItemService
{
    Task<List<Item>> GetAllItemsAsync();
    Task<Item?> GetItemByIdAsync(int id);
    Task<Item> AddItemAsync(Item item);
    Task<Item> UpdateItemAsync(Item item);
    Task DeleteItemAsync(int id);
}
