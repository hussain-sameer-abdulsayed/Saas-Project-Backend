using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SassProject.Data;
using SassProject.Dtos.ProductRepo;
using SassProject.IRepos;
using SassProject.Models;
using SassProject.Repos;

namespace SassProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class productsController : ControllerBase
    {
        private readonly Context _context;
        private readonly IProductRepo _productRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IJWTMangerRepo _jWTMangerRepo;
        public productsController(Context context, IProductRepo productRepo, ITransactionRepo transactionRepo, IJWTMangerRepo jWTMangerRepo)
        {
            _context = context;
            _productRepo = productRepo;
            _transactionRepo = transactionRepo;
            _jWTMangerRepo = jWTMangerRepo;
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

        [HttpGet("Search")]
        public async Task<IActionResult> SearchProducts(string query)
        {
            try
            {
                var products = await _productRepo.SearchProducts(query);
                if (!products.Any())
                {
                    return NotFound("There is not products in this name");
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                var userId = _jWTMangerRepo.GetUserId(token);
                */
                var userId = "0842a1a0-44d2-4882-8266-12e5a939d452";

                await _transactionRepo.BeginTransactionAsync();
                var isCreated = await _productRepo.CreateProductAsync(userId, productDto);
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
                return BadRequest($"An error occurred while trying to create product : {ex.Message}");
            }
        }


        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync(int productId, UpdateProductDto productDto)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var isUpdated = await _productRepo.UpdateProductAsync(productId, productDto);
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
                return BadRequest($"An error occurred while trying to update product  : {ex.Message}");
            }
        }


        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var isDeleted = await _productRepo.DeleteProductAsync(productId);
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
                return BadRequest($"An error occurred while trying to delete product : {ex.Message}");
            }
        }
    }
}
