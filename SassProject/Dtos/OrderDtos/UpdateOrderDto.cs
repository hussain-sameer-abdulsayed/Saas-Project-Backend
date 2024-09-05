using SassProject.Models;

namespace SassProject.Dtos.OrderDtos
{
    public class UpdateOrderDto
    {
        public string? UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? NearestPoint { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;


        public State? State { get; set; }

    }
}
