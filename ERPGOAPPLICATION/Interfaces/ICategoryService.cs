using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> AddCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(int id);


}
