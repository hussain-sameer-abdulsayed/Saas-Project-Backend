using SassProject.Models;

namespace SassProject.Dtos.CategoryDto
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? MainImage { get; set; }
    }
}
