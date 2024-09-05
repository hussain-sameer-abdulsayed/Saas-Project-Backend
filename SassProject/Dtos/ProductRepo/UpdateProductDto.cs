using SassProject.Models;

namespace SassProject.Dtos.ProductRepo
{
    public class UpdateProductDto
    {
        public string? Name { get; set; } = string.Empty;
        public double? Price { get; set; }
        public int? StockQuantity { get; set; }
        public string? Description { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
    }
}