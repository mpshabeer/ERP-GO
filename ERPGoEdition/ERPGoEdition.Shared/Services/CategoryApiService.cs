using ERPGOAPPLICATION.Interfaces;
using ERPGODomain.Entities;

namespace ERPGoEdition.Shared.Services;

public class CategoryApiService : ICategoryService
{
    private readonly ICategoryApiClient _apiClient;

    public CategoryApiService(ICategoryApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<List<Category>> GetAllCategoriesAsync() => _apiClient.GetAllCategoriesAsync();
    public Task<Category?> GetCategoryByIdAsync(int id) => _apiClient.GetCategoryByIdAsync(id);
    public Task<Category> AddCategoryAsync(Category category) => _apiClient.AddCategoryAsync(category);
    public Task<Category> UpdateCategoryAsync(Category category) => _apiClient.UpdateCategoryAsync(category);
    public Task DeleteCategoryAsync(int id) => _apiClient.DeleteCategoryAsync(id);
}
