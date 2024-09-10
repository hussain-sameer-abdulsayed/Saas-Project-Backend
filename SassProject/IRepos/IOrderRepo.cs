using SassProject.Dtos.OrderDtos;
using SassProject.Models;
using static NuGet.Packaging.PackagingConstants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SassProject.IRepos
{
    public interface IOrderRepo
    {
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByUserPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByStateAsync(State state);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int numberOfDays);
        Task<double> GetTotalSalesAsync(); // return orders Total that have completed status to avoid canceled orders
        Task<IEnumerable<Order>> GetOrdersWithItemsAsync();

        // Retrieves a summary of orders within a specified date range, possibly including metrics like total sales, number of orders, etc.
        Task<OrderSummary> GetOrderSummaryAsync(DateTime startDate, DateTime endDate);

        //  Retrieves the top n products based on the number of orders or total sales amount.
        Task<IEnumerable<Product>> GetTopSellingProductsAsync(int topN);
        Task<CheckFunc> CreateOrderAsync(CreateOrderDto orderDto);
        Task<CheckFunc> UpdateOrderAsync(string orderId,UpdateOrderDto orderDto);
        Task<CheckFunc> UpdateOrderItemsAsync(string orderId, List<UpdateOrderItem> orderItemsDto);
        Task<CheckFunc> UpdateOrderStatusAsync(string orderId, OrderStatus status);
        Task<CheckFunc> DeleteOrderAsync(string orderId);
        Task<CheckFunc> CancelOrderAsync(string orderId);
    }
}
