using System.ComponentModel.DataAnnotations;

namespace SassProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string MainImage { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
        public string UpdatedAt { get; set; } = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"); // Initialize with CreatedAt


        public Category Category { get; set; } // Navigation property
        public int CategoryId { get; set; } // Foreign key property

        public User CreatedByUser { get; set; } // Navigation property
        public string CreatedByUserId { get; set; } // Foreign key property
    }

}
