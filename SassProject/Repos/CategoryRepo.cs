using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SassProject.Data;
using SassProject.Dtos.CategoryDto;
using SassProject.IRepos;
using SassProject.Models;
using SassProject.Repos;

namespace SassProject.Repos
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly IimageRepo _imageRepo;
        public CategoryRepo(Context context, IMapper mapper, IimageRepo imageRepo)
        {
            _context = context;
            _mapper = mapper;
            _imageRepo = imageRepo;
        }


        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var categories = await _context.Categories
                .Select(c =>  new Category 
                { 
                    Id = c.Id, 
                    Name = c.Name,
                    MainImage = c.MainImage,

                }).ToListAsync();

            // get images for each category
            if (categories.Count() != 0)
            {
                foreach (var category in categories)
                {
                    //var uniqueFileName = await _imageRepo.GetCategoryUniqueFileNameAsync(category.Id);
                    if (category.MainImage != null)
                    {
                        category.MainImage = _imageRepo.GetImage(category.MainImage);
                    }
                    else
                    {
                        category.MainImage = "https://localhost:7249/images/categorydefualt.png";
                    }
                }
            }
            
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return null;
            }

            // Fetch the main image unique file name
            //var uniqueFileName = await _imageRepo.GetCategoryUniqueFileNameAsync(categoryId);
            if (category.MainImage == null)
            {
                // Set a default image if no unique file name is found
                category.MainImage = "https://localhost:7249/images/categorydefualt.png";
            }
            else
            {
                // Fetch the main image URL if a unique file name exists
                category.MainImage = _imageRepo.GetImage(category.MainImage);
            }


            return category;
        }

        public async Task<CheckFunc> CreateCategoryAsync(string userId, CreateCategoryDto categoryDto)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new CheckFunc { Message = $"An error occurred while creating the category : User with ID {userId} not found." };
                }
                category.CreatedByUser = user;
                category.CreatedByUserId = userId;

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                if(categoryDto.MainImage is not null)
                {
                    // add image
                    _context.Categories.Attach(category);
                    string uniqueFileName = await _imageRepo.SaveUploadedFileAsync(categoryDto.MainImage);
                    category.MainImage = uniqueFileName;
                    await _context.SaveChangesAsync();
                }
                return new CheckFunc { IsSucceeded = true, Message = "category created successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An error occurred while creating the category : {ex.Message}" };
            }
        }

        public async Task<CheckFunc> UpdateCategoryAsync(int categoryId,UpdateCategoryDto categoryDto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    return new CheckFunc { Message = $"An error occurred while updating the category : category with ID {categoryId} not found." };
                }
                _context.Attach(category);
                if (!string.IsNullOrEmpty(categoryDto.Description))
                {
                    category.Description = categoryDto.Description;
                }
                if (!string.IsNullOrEmpty(categoryDto.Name))
                {
                    category.Name = categoryDto.Name;
                }
                category.UpdatedAt = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true , Message = "category updated successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An error occurred while updating the category : {ex.Message}" };
            }
        }

        public async Task<CheckFunc> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    return new CheckFunc { Message = $"An error occurred while deleting the category : Category with ID {categoryId} not found." };
                }
                // get file to delete it
                string uniqueFileName = category.MainImage;
                
                _context.Attach(category);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                // delete image(file)
                _imageRepo.DeleteFile(uniqueFileName);

                return new CheckFunc { IsSucceeded = true, Message = "Category Deleted Successfuly" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An error occurred while deleting the category : {ex.Message}" };
            }
        }


    }
}
