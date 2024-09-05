using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SassProject.Data;
using SassProject.Dtos.ProductRepo;
using SassProject.IRepos;
using SassProject.Models;
using SassProject.Repos;

namespace SassProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly Context _context;
        private readonly IProductRepo _productRepo;
        public ProductController(Context context, IProductRepo productRepo)
        {
            _context = context;
            _productRepo = productRepo;
        }


        [HttpGet()]
        public async Task<IActionResult> GetProductsAsync()
        {
            try
            {
                var products = await _productRepo.GetProductsAsync();
                if (!products.Any())
                {
                    return NotFound("There is no products");
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while trying to get products  : {ex.Message}");
            }
        }


        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategoryId(int categoryId)
        {
            try
            {
                var products = await _productRepo.GetProductsByCategoryIdAsync(categoryId);
                if (!products.Any())
                {
                    return NotFound("There is no products");
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while trying to get products  : {ex.Message}");
            }
        }


        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _productRepo.GetProductByIdAsync(productId);
                if(product == null)
                {
                    return NotFound("There is no product in this id");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost()]
        public async Task<IActionResult> CreateProductAsync(CreateProductDto productDto)
        {
            try
            {
                /*
                string accessToken = Request.Headers[HeaderNames.Authorization];
                var token = accessToken.Substring(7);
                var userId = _jwtManagerRepo.GetUserId(token);
                */
                var userId = "0842a1a0-44d2-4882-8266-12e5a939d452";

                var isCreated = await _productRepo.CreateProductAsync(userId, productDto);
                if (!isCreated.IsSucceeded)
                {
                    return BadRequest(isCreated.Message);
                }
                return Ok(isCreated.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while trying to create product : {ex.Message}");
            }
        }


        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync(int productId, UpdateProductDto productDto)
        {
            try
            {
                var isUpdated = await _productRepo.UpdateProductAsync(productId, productDto);
                if (!isUpdated.IsSucceeded)
                {
                    return BadRequest(isUpdated.Message);
                }
                return Ok(isUpdated.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while trying to update product  : {ex.Message}");
            }
        }


        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            try
            {
                var isDeleted = await _productRepo.DeleteProductAsync(productId);
                if (!isDeleted.IsSucceeded)
                {
                    return BadRequest(isDeleted.Message);
                }
                return Ok(isDeleted.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while trying to delete product : {ex.Message}");
            }
        }
    }
}
