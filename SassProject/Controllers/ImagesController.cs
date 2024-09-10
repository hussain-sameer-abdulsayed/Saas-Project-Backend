using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SassProject.IRepos;

namespace SassProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IimageRepo _imageRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IProductRepo _productRepo;
        private readonly ITransactionRepo _transactionRepo;

        public ImagesController(IimageRepo imageRepo, ICategoryRepo categoryRepo, IProductRepo productRepo, ITransactionRepo transactionRepo)
        {
            _imageRepo = imageRepo;
            _categoryRepo = categoryRepo;
            _productRepo = productRepo;
            _transactionRepo = transactionRepo;
        }


        [HttpPatch("category/{categoryId}")]
        public async Task<IActionResult> UpdateCategoryImage(int categoryId, IFormFile image)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var isUpdated = await _imageRepo.UpdateCategoryMainImage(categoryId, image);
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
                return BadRequest(ex.Message);
            }
        }
        

        [HttpPatch("product/{productId}")]
        public async Task<IActionResult> UpdateProductImage(int productId, IFormFile image)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var isUpdated = await _imageRepo.UpdateProductMainImage(productId, image);
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
                return BadRequest(ex.Message);
            }
        }
        
    }
}
