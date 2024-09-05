namespace SassProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MainImage { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
        public string UpdatedAt { get; set; } = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");



        public List<Product>? Products { get; set; } = new List<Product>();
        public User? CreatedByUser { get; set; }
        public string? CreatedByUserId { get; set;} = string.Empty;
    }
}
