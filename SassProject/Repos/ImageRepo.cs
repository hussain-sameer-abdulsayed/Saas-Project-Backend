using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SassProject.Data;
using SassProject.IRepos;

namespace SassProject.Repos
{
    public class ImageRepo : IimageRepo
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ImageRepo(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<CheckFunc> UpdateCategoryMainImage(int categoryId, IFormFile file)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return new CheckFunc { Message = "Category was not found" };
            }
            DeleteFile(category.MainImage); // this to delete the old file form projects images


            var uniqueFileName = await SaveUploadedFileAsync(file);
            if (uniqueFileName == null || uniqueFileName == "Image size large")
            {
                return new CheckFunc { Message = "Image size large" };
            }
            category.MainImage = uniqueFileName;
            await _context.SaveChangesAsync();
            return new CheckFunc { IsSucceeded = true , Message = "Category Main Image updated successfully"};
        }

        public async Task<CheckFunc> UpdateProductMainImage(int productId, IFormFile file)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return new CheckFunc { Message = "product was not found" };
            }
            DeleteFile(product.MainImage); // this to delete the old file form projects images


            var uniqueFileName = await SaveUploadedFileAsync(file);
            if (uniqueFileName == null || uniqueFileName == "Image size large")
            {
                return new CheckFunc { Message = "Image size large" };
            }
            _context.Products.Attach(product);
            product.MainImage = uniqueFileName;
            await _context.SaveChangesAsync();
            return new CheckFunc { IsSucceeded = true, Message = "product Main Image updated successfully" };
        }

        public async Task<string> SaveUploadedFileAsync(IFormFile file)
        {
            // Check if a file was uploaded
            if (file != null && file.Length > 0)
            {
                const int maxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB
                if (file.Length > maxFileSizeInBytes)
                {
                    return ("Image size large");
                }
                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + ".png";

                // Construct the file path
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the file system
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return the unique file name
                return uniqueFileName;
            }
            else
            {
                // Return null if no file was uploaded
                return null;
            }
        }
        
        public async Task<string> GetCategoryUniqueFileNameAsync(int categoryId)
        {
            try
            {
                var uniqueFileName = await _context.Categories
                    .Where(x => x.Id == categoryId)
                    .Select(x => x.MainImage)
                    .FirstOrDefaultAsync();

                return uniqueFileName;
            }
            catch
            {
                // add logging 
                return null;
            }
        }
        
        public string GetImage(string uniqueFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(uniqueFileName))
                {
                    return null;
                }
                const string _baseImageUrl = "https://localhost:7249/images/";
                string imageUrl = $"{_baseImageUrl}{uniqueFileName}";
                return imageUrl;
            }
            catch
            {
                return null;
            }
        }

        public async Task<CheckFunc> DeleteCategoryMainImage(int categoryId)
        {
            try
            {
                string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = await GetCategoryUniqueFileNameAsync(categoryId);
                if (uniqueFileName == null)
                {
                    return new CheckFunc { Message = "An error occurred while deleting the image : There is no image" };
                }
                // Construct the full file path based on the unique file name and the images directory
                string filePath = Path.Combine(Directory, uniqueFileName);

                // Check if the file exists before attempting to delete it
                if (File.Exists(filePath))
                {
                    // Delete the file
                    File.Delete(filePath);
                    var category = await _context.Categories.FindAsync(categoryId);
                    category.MainImage = null;
                    _context.Attach(category);
                    await _context.SaveChangesAsync();
                    return new CheckFunc { IsSucceeded = true, Message = "The image was deleted successfuly" }; // Return true if the file was successfully deleted
                }
                else
                {
                    return new CheckFunc { Message = "An error occurred while deleting the image : The image was not deleted successfuly " }; ;
                }
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An error occurred while deleting the image : {ex.Message}" }; ;
            }
        }
        public void DeleteFile(string uniqueFileName)
        {
            string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string filePath = Path.Combine(Directory, uniqueFileName);

            // Check if the file exists before attempting to delete it
            if (File.Exists(filePath))
            {
                // Delete the file
                File.Delete(filePath);
            }
        }
    }
}
