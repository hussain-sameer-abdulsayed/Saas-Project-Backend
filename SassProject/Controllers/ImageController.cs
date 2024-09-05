using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SassProject.IRepos;

namespace SassProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IimageRepo _imageRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IProductRepo _productRepo;

        public ImageController(IimageRepo imageRepo, ICategoryRepo categoryRepo, IProductRepo productRepo)
        {
            _imageRepo = imageRepo;
            _categoryRepo = categoryRepo;
            _productRepo = productRepo;
        }


        [HttpPatch("Category/{categoryId}")]
        public async Task<IActionResult> UpdateCategoryImage(int categoryId, IFormFile image)
        {
            try
            {
                var isUpdated = await _imageRepo.UpdateCategoryMainImage(categoryId, image);
                if (!isUpdated.IsSucceeded)
                {
                    return BadRequest(isUpdated.Message);
                }

                return Ok(isUpdated.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpPatch("Product/{productId}")]
        public async Task<IActionResult> UpdateProductImage(int productId, IFormFile image)
        {
            try
            {
                var isUpdated = await _imageRepo.UpdateProductMainImage(productId, image);
                if (!isUpdated.IsSucceeded)
                {
                    return BadRequest(isUpdated.Message);
                }

                return Ok(isUpdated.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
