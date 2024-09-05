namespace SassProject.IRepos
{
    public interface IimageRepo
    {
        Task<string> SaveUploadedFileAsync(IFormFile file);
        Task<CheckFunc> UpdateCategoryMainImage(int categoryId, IFormFile file);
        Task<CheckFunc> UpdateProductMainImage(int productId, IFormFile file);
        Task<string> GetCategoryUniqueFileNameAsync(int categoryId);
        
        string GetImage(string uniqueFileName);
        Task<CheckFunc> DeleteCategoryMainImage(int categoryId); // if category has many images here must pass imageUrl that need to delete
        void DeleteFile(string uniqueFileName);
    }
}
