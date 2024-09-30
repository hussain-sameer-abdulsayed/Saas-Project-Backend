using SassProject.Dtos.CategoryDto;
using SassProject.Dtos.ProductRepo;
using SassProject.Models;

namespace SassProject.IRepos
{
    public interface IProductRepo
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> SearchProducts(string query);
        Task<CheckFunc> CreateProductAsync(string userId, CreateProductDto productDto);
        Task<CheckFunc> UpdateProductAsync(int productId, UpdateProductDto productDto);
        Task<CheckFunc> DeleteProductAsync(int productId);
    }
}
