using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SassProject.Data;
using SassProject.Dtos.ProductRepo;
using SassProject.IRepos;
using SassProject.Models;

namespace SassProject.Repos
{
    public class ProductRepo : IProductRepo
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly IimageRepo _imageRepo;

        public ProductRepo(Context context, IMapper mapper, IimageRepo imageRepo)
        {
            _context = context;
            _mapper = mapper;
            _imageRepo = imageRepo;
        }


        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = await _context.Products
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryId = p.CategoryId,
                    MainImage = p.MainImage,
                    Price = p.Price
                }).ToListAsync();
            if(products.Any())
            {
                foreach(var product in products)
                {
                    //string uniqueFileName = await _imageRepo.GetCategoryUniqueFileNameAsync(product.Id);
                    if(product.MainImage != null)
                    {
                        product.MainImage = _imageRepo.GetImage(product.MainImage);
                    }
                    else
                    {
                        product.MainImage = "https://localhost:7249/images/productdefualt.png";
                    }
                }
            }


            return products;
        }


        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if(product == null)
            {
                return null;
            }

            // get image
            if(product.MainImage != null)
            {
                product.MainImage = _imageRepo.GetImage(product.MainImage);
            }
            else
            {
                product.MainImage = "https://localhost:7249/images/productdefualt.png";
            }

            return product;
        }


        public async Task<CheckFunc> CreateProductAsync(string userId, CreateProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                var category = await _context.Categories.FindAsync(productDto.CategoryId);
                if(category == null)
                {
                    return new CheckFunc { Message = $"An error occurred while creating the product : Category with ID {productDto.CategoryId} not found." };
                }
                product.Category = category;
                product.CategoryId = productDto.CategoryId;

                var user = await _context.Users.FindAsync(userId);
                if(user == null)
                {
                    return new CheckFunc { Message = $"An error occurred while creating the product : User with ID {userId} not found." };
                }
                product.CreatedByUser = user;
                product.CreatedByUserId = userId;

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();


                if (productDto.MainImage is not null)
                {
                    // add image
                    _context.Products.Attach(product);
                    string uniqueFileName = await _imageRepo.SaveUploadedFileAsync(productDto.MainImage);
                    product.MainImage = uniqueFileName;
                    await _context.SaveChangesAsync();
                }
                return new CheckFunc { IsSucceeded = true, Message = "product created successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An error occurred while creating the product : {ex.Message}" };
            }
        }
        

        public async Task<CheckFunc> UpdateProductAsync(int productId, UpdateProductDto productDto)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return new CheckFunc { Message = $"An error occurred while updating the product : product with ID {productId} not found." };
                }
                _context.Attach(product);
                if (!string.IsNullOrEmpty(productDto.Description))
                {
                    product.Description = productDto.Description;
                }
                if (!string.IsNullOrEmpty(productDto.Name))
                {
                    product.Name = productDto.Name;
                }
                if(productDto.Price.HasValue &&  productDto.Price.Value != 0.0)
                {
                    product.Price = productDto.Price.Value;
                }
                if(productDto.StockQuantity.HasValue && productDto.StockQuantity.Value != 0)
                {
                    product.StockQuantity = productDto.StockQuantity.Value;
                }
                if(productDto.CategoryId.HasValue && productDto.CategoryId.Value != 0)
                {
                    var category = await _context.Categories.FindAsync(productId);
                    if (category == null)
                    {
                        return new CheckFunc { Message = $"An error occurred while updating the product: Category with ID {productDto.CategoryId.Value} not found." };
                    }
                    product.CategoryId = productDto.CategoryId.Value;
                }
                product.UpdatedAt = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true, Message = "product updated successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An error occurred while updating the product : {ex.Message}" };
            }
        }


        public async Task<CheckFunc> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return new CheckFunc { Message = $"An error occurred while deleting the product : product with ID {productId} not found." };
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true, Message = "product deleted successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An error occurred while deleting the product : {ex.Message}" };
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Select(x => new Product
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    Name = x.Name,
                    Price = x.Price,
                    MainImage = x.MainImage
                }).ToListAsync();

            // include images
            foreach (var product in products)
            {
                if (product.MainImage != null)
                {
                    product.MainImage = _imageRepo.GetImage(product.MainImage);
                }
                else
                {
                    product.MainImage = "https://localhost:7249/images/productdefualt.png";
                }
            }

            return products;
        }
    }
}
