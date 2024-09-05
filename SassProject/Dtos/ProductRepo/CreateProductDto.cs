using SassProject.Models;

namespace SassProject.Dtos.ProductRepo
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int StockQuantity { get; set; } 
        public string Description { get; set; } = string.Empty;
        public IFormFile MainImage { get; set; }
        public int CategoryId { get; set; }
    }
}
