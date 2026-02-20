using ERPGODomain.Entities;
using Refit;

namespace ERPGoEdition.Shared.Services;

public interface ICategoryApiClient
{
    [Get("/api/categories")]
    Task<List<Category>> GetAllCategoriesAsync();

    [Get("/api/categories/{id}")]
    Task<Category?> GetCategoryByIdAsync(int id);

    [Post("/api/categories")]
    Task<Category> AddCategoryAsync([Body] Category category);

    [Put("/api/categories")]
    Task<Category> UpdateCategoryAsync([Body] Category category);

    [Delete("/api/categories/{id}")]
    Task DeleteCategoryAsync(int id);
}
