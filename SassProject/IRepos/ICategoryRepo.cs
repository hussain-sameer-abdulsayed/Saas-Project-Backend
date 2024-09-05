using SassProject.Dtos.CategoryDto;
using SassProject.Models;

namespace SassProject.IRepos
{
    public interface ICategoryRepo
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
        //Task<Category> GetCategoryByNameAsync(string name); search
        Task<CheckFunc> CreateCategoryAsync(string userId,CreateCategoryDto categoryDto);
        Task<CheckFunc> UpdateCategoryAsync(int categoryId, UpdateCategoryDto categoryDto);
        Task<CheckFunc> DeleteCategoryAsync(int categoryId);

    }
}
