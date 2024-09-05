using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SassProject.Models
{
    public class OrderItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; } 

        public void CalcTotal()
        {
            if(Product != null)
            {
                UnitPrice = Product.Price;
                Total = UnitPrice * Quantity;
            }
        }




        public Product Product { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public Order Order { get; set; }
        [ForeignKey(nameof(Order))]
        public string OrderId { get; set; }
    }

}
