using SassProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SassProject.Dtos.OrderDtos
{
    public class UpdateOrderItem
    {
        public string? OrderItemId { get; set; } = "0";
        public int Quantity { get; set; }
        public int ProductId { get; set; }

    }
}
