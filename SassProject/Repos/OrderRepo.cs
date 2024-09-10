using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SassProject.Data;
using SassProject.Dtos.OrderDtos;
using SassProject.IRepos;
using SassProject.Models;

namespace SassProject.Repos
{
    public class OrderRepo : IOrderRepo
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public OrderRepo(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders
                                      .Include(o => o.OrderItems)
                                      .FirstOrDefaultAsync(o => o.Id == orderId);


        }


        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            // what i have to return here all order data or a sprecific cloumns
            return await _context.Orders.ToListAsync();

        }


        public async Task<IEnumerable<Order>> GetOrdersByStateAsync(State state)
        {
            return await _context.Orders
                             .Where(o => o.State == state)
                             .ToListAsync();
        }


        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                             .Where(o => o.OrderStatus == status)
                             .ToListAsync();
        }


        public async Task<IEnumerable<Order>> GetOrdersByUserPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Orders
                             .Where(o => o.PhoneNumber == phoneNumber)
                             .ToListAsync();
        }


        public async Task<OrderSummary> GetOrderSummaryAsync(DateTime startDate, DateTime endDate)
        {
            // Load Orders with related OrderItems and Product
            var orders = await _context.Orders
                                       .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                                       .Include(o => o.OrderItems) // Include OrderItems
                                       .ThenInclude(oi => oi.Product) // Then Include Product for OrderItems
                                       .ToListAsync();

            var totalSales = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count();
            var totalItemsSold = orders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity); // Should now load OrderItems
            var averageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;
            var mostPopularProduct = orders.SelectMany(o => o.OrderItems)
                                           .GroupBy(oi => oi.ProductId)
                                           .OrderByDescending(g => g.Count())
                                           .Select(g => g.FirstOrDefault()?.Product.Name) // Ensure Product is loaded
                                           .FirstOrDefault();

            var orderStatusCounts = orders.GroupBy(o => o.OrderStatus)
                                          .ToDictionary(g => g.Key, g => g.Count());

            return new OrderSummary
            {
                TotalSales = totalSales,
                TotalOrders = totalOrders,
                TotalItemsSold = totalItemsSold,
                AverageOrderValue = averageOrderValue,
                MostPopularProduct = mostPopularProduct,
                OrderStatusCounts = orderStatusCounts,
                StartDate = startDate,
                EndDate = endDate,
                TopCustomers = orders.GroupBy(o => o.PhoneNumber)
                                     .OrderByDescending(g => g.Sum(o => o.TotalAmount)).Take(5)
                             .Select(g => g.Key)
                             .ToList()
            };
        }




        public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync()
        {
            return await _context.Orders
                             .Include(o => o.OrderItems)
                             .ToListAsync();
        }


        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int numberOfDays)
        {
            var recentDate = DateTime.Now.AddDays(-numberOfDays);
            return await _context.Orders
                                 .Where(o => o.CreatedAt >= recentDate)
                                 .ToListAsync();
        }


        public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int topN)
        {
            return await _context.OrderItems
                         .GroupBy(oi => oi.Product.Id)
                         .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                         .Take(topN)
                         .Select(g => g.First().Product)
                         .ToListAsync();
        }


        public async Task<double> GetTotalSalesAsync()
        {
            return await _context.Orders
                             .Where(o => o.OrderStatus == OrderStatus.COMPLETED)
                             .SumAsync(o => o.TotalAmount);
        }


        public async Task<CheckFunc> CreateOrderAsync(CreateOrderDto orderDto)
        {
            try
            {
                Order order = new Order
                {
                    UserName = orderDto.UserName,
                    PhoneNumber = orderDto.PhoneNumber,
                    State = orderDto.State,
                    City = orderDto.City,
                    TotalAmount = 0,
                    OrderItems = new List<OrderItem>(),
                };

                //do not add order.id add only order cause the order was not saved in the database yet

                List<OrderItem> orderItems = new List<OrderItem>();
                foreach (var item in orderDto.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {

                        if (product.StockQuantity < item.Quantity)
                        {
                            return new CheckFunc { IsSucceeded = false, Message = $"Insufficient stock for product {product.Name}" };
                        }


                        var orderitem = new OrderItem
                        {
                            Product = product,
                            ProductId = item.ProductId,
                            Order = order,
                            //OrderId = order.Id, // delete this
                            Quantity = item.Quantity,
                            UnitPrice = product.Price,
                            Total = 0
                        };
                        orderitem.CalcTotal(); // this method calc the total price of items
                        orderItems.Add(orderitem);
                    }
                }
                order.OrderItems = orderItems;



                if (orderDto.Email != null)
                {
                    order.Email = orderDto.Email;
                }
                if (orderDto.Notes != null)
                {
                    order.Notes = orderDto.Notes;
                }
                if (orderDto.NearestPoint != null)
                {
                    order.NearestPoint = orderDto.NearestPoint;
                }

                // Update inventory
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.Product.Id);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;
                        _context.Products.Update(product);
                    }
                }

                order.CalculateTotalAmount(); // this method for calc total price of an order

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                return new CheckFunc { IsSucceeded = true, Message = "Order Created Successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An Error Cccurred While Creating The Order : {ex.Message}" };
            }
        }


        public async Task<CheckFunc> UpdateOrderAsync(string orderId, UpdateOrderDto orderDto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return new CheckFunc { Message = $"An Error Cccurred While Updating The Order : There is no order with this ID {orderId}" };
                }

                if (!string.IsNullOrEmpty(orderDto.UserName))
                {
                    order.UserName = orderDto.UserName;
                }
                if (!string.IsNullOrEmpty(orderDto.Email))
                {
                    order.Email = orderDto.Email;
                }
                if (!string.IsNullOrEmpty(orderDto.PhoneNumber))
                {
                    order.PhoneNumber = orderDto.PhoneNumber;
                }
                if (!string.IsNullOrEmpty(orderDto.City))
                {
                    order.City = orderDto.City;
                }
                if (!string.IsNullOrEmpty(orderDto.NearestPoint))
                {
                    order.NearestPoint = orderDto.NearestPoint;
                }
                if (!string.IsNullOrEmpty(orderDto.Notes))
                {
                    order.Notes = orderDto.Notes;
                }
                if (orderDto.State != null)
                {
                    order.State = orderDto.State.Value;
                }
                await _context.SaveChangesAsync();

                return new CheckFunc { IsSucceeded = true, Message = "Order Updated Successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An Error Cccurred While Updating The Order : {ex.Message}" };
            }
        }

        public async Task<CheckFunc> UpdateOrderStatusAsync(string orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true, Message = "Order Status Updated Successfully" };
            }
            return new CheckFunc { Message = $"An Error Cccurred While Updating The Order Status : There is no order with is ID {orderId}" };
        }


        public async Task<CheckFunc> DeleteOrderAsync(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true, Message = "Order Deleted Successfully" };
            }
            return new CheckFunc { Message = $"An Error Cccurred While Deleting The Order : There is no order with is ID {orderId}" };
        }


        public async Task<CheckFunc> CancelOrderAsync(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.CANCELLED;
                await _context.SaveChangesAsync();

                // Revert inventory if needed
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.Product.Id);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        _context.Products.Update(product);
                    }
                }
                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true, Message = "Order Cancelled Successfully" };
            }
            return new CheckFunc { Message = $"An Error Cccurred While Creating The Order : There is no order with is Order Id {orderId}" };
        }

        public async Task<CheckFunc> UpdateOrderItemsAsync(string orderId, List<UpdateOrderItem> orderItemsDto)
        {
            try
            {
                // Find the order with its order items
                var order = await _context.Orders
                                  .Include(o => o.OrderItems)  // Include existing order items
                                  .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new CheckFunc { Message = $"Order with ID {orderId} was not found." };
                }

                var productsToUpdate = new Dictionary<int, int>();

                foreach (var updatedItem in orderItemsDto)
                {
                    var product = await _context.Products.FindAsync(updatedItem.ProductId);

                    if (product == null)
                    {
                        return new CheckFunc { Message = $"Product with ID {updatedItem.ProductId} was not found." };
                    }

                    // Check for stock availability for the new or updated quantity
                    if (product.StockQuantity < updatedItem.Quantity)
                    {
                        return new CheckFunc { Message = $"Insufficient stock for product {product.Name}." };
                    }


                    // in frontEnd if we add a new item(product) to the order send in orderitemId = "0"
                    if (updatedItem.OrderItemId == "0")
                    {
                        // New OrderItem, create and add it
                        var newOrderItem = new OrderItem
                        {
                            ProductId = updatedItem.ProductId,
                            Product = product,
                            Quantity = updatedItem.Quantity,
                            UnitPrice = product.Price,
                            Order = order,
                            OrderId = orderId
                        };
                        newOrderItem.CalcTotal();

                        order.OrderItems.Add(newOrderItem);

                        // Track product quantity change
                        if (!productsToUpdate.ContainsKey(product.Id))
                        {
                            productsToUpdate[product.Id] = 0;
                        }
                        productsToUpdate[product.Id] -= updatedItem.Quantity;
                    }
                    else
                    {
                        // Update existing OrderItem
                        var orderItem = order.OrderItems.FirstOrDefault(oi => oi.Id == updatedItem.OrderItemId);

                        if (orderItem != null)
                        {
                            // Track the change in quantity
                            int quantityChange = updatedItem.Quantity - orderItem.Quantity;

                            // Check if stock is sufficient after considering the change
                            if (product.StockQuantity < quantityChange)
                            {
                                return new CheckFunc { Message = $"Insufficient stock for product {product.Name}." };
                            }

                            // Update the existing order item
                            orderItem.Quantity = updatedItem.Quantity;
                            orderItem.CalcTotal();

                            // Track product quantity change
                            if (!productsToUpdate.ContainsKey(product.Id))
                            {
                                productsToUpdate[product.Id] = 0;
                            }
                            productsToUpdate[product.Id] -= quantityChange;
                        }
                    }
                }

                // Update product stock based on changes
                foreach (var kvp in productsToUpdate)
                {
                    var product = await _context.Products.FindAsync(kvp.Key);
                    if (product != null)
                    {
                        product.StockQuantity += kvp.Value;
                        _context.Products.Update(product);
                    }
                }

                // Recalculate order total amount
                order.TotalAmount = 0;
                order.CalculateTotalAmount();

                // Save changes
                await _context.SaveChangesAsync();

                return new CheckFunc { IsSucceeded = true, Message = "Order items updated successfully." };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"An error occurred while updating order items: {ex.Message}" };
            }
        }



    }
}