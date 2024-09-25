using SassProject.Models;

namespace SassProject.Dtos.OrderDtos
{
    public class CreateOrderDto
    {
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? NearestPoint { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
        public string? State { get; set; }

        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }
}
