using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Net.Http.Headers;
using SassProject.Data;
using SassProject.Dtos.CategoryDto;
using SassProject.IRepos;
using SassProject.Models;

namespace SassProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("concurrencyPolicy")]
    public class CategoriesController : ControllerBase
    {
        private readonly Context _context;
        private readonly IimageRepo _imageRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IJWTMangerRepo _jWTMangerRepo;
        private readonly ITransactionRepo _transactionRepo;

        public CategoriesController(Context context, IimageRepo imageRepo, ICategoryRepo categoryRepo, IJWTMangerRepo jWTMangerRepo, ITransactionRepo transactionRepo)
        {
            _context = context;
            _imageRepo = imageRepo;
            _categoryRepo = categoryRepo;
            _jWTMangerRepo = jWTMangerRepo;
            _transactionRepo = transactionRepo;
        }

        
        [HttpGet()]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepo.GetCategoriesAsync();
                if (!categories.Any())
                {
                    return NotFound("There is no categories");
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while trying to get all categories  : {ex.Message}");
            }
        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                var category = await _categoryRepo.GetCategoryByIdAsync(categoryId);
                if(category == null)
                {
                    return NotFound("category was not found");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while trying to get this category  : {ex.Message}");
            }
        }

        //[Authorize(Roles = "SUPERADMIN, ADMIN")]
        [HttpPost()]
        public async Task<IActionResult> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            try
            {
                /*
                string accessToken = Request.Headers[HeaderNames.Authorization];
                var token = accessToken.Substring(7);
                var userId = _jWTMangerRepo.GetUserId(token);
                */
                var userId = "0842a1a0-44d2-4882-8266-12e5a939d452";
                await _transactionRepo.BeginTransactionAsync();
                var isCreated = await _categoryRepo.CreateCategoryAsync(userId, categoryDto);
                if (!isCreated.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(isCreated.Message);
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(isCreated.Message);
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest($"An error occurred while trying to create category  : {ex.Message}");
            }
        }


        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategoryAsync(int categoryId,UpdateCategoryDto categoryDto)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var isUpdated = await _categoryRepo.UpdateCategoryAsync(categoryId, categoryDto);
                if (!isUpdated.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(isUpdated.Message);
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(isUpdated.Message);
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest($"An error occurred while trying to update category  : {ex.Message}");
            }
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var isDeleted = await _categoryRepo.DeleteCategoryAsync(categoryId);
                if (!isDeleted.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(isDeleted.Message);
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(isDeleted.Message);
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest($"An error occurred while trying to delete category  : {ex.Message}");
            }
        }
    }
}
