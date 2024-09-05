using SassProject.Models;

namespace SassProject.Dtos.OrderDtos
{
    public class OrderSummary
    {
        public int TotalOrders { get; set; }
        public double TotalSales { get; set; }
        public int TotalItemsSold { get; set; }
        public double AverageOrderValue { get; set; }
        public string MostPopularProduct { get; set; }
        public Dictionary<OrderStatus, int> OrderStatusCounts { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> TopCustomers { get; set; } = new List<string>();

        public OrderSummary()
        {
            OrderStatusCounts = new Dictionary<OrderStatus, int>();
        }
    }
}
