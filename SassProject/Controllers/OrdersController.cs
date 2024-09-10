using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
using SassProject.Dtos.OrderDtos;
using SassProject.IRepos;
using SassProject.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static NuGet.Packaging.PackagingConstants;
using State = SassProject.Models.State;

namespace SassProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepo _orderRepo;
        private readonly ITransactionRepo _transactionRepo;

        public OrdersController(IOrderRepo orderRepo, ITransactionRepo transactionRepo)
        {
            _orderRepo = orderRepo;
            _transactionRepo = transactionRepo;
        }


        [HttpGet()]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await _orderRepo.GetOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            try
            {
                var order = await _orderRepo.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound(new { Message = $"Order with ID {orderId} not found." });
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("byState/{state}")]
        public async Task<IActionResult> GetOrdersByState(State state)
        {
            try
            {
                var orders = await _orderRepo.GetOrdersByStateAsync(state);
                if (!orders.Any())
                {
                    return NotFound(new { Message = $"Orders with state {state} not found." });
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("byStatus/{status}")]
        public async Task<IActionResult> GetOrdersByStatus(OrderStatus status)
        {
            try
            {
                var orders = await _orderRepo.GetOrdersByStatusAsync(status);
                if (!orders.Any())
                {
                    return NotFound(new { Message = $"Orders with status {status} not found." });
                }
                return Ok(orders);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("byPhoneNumber/{phoneNumber}")]
        public async Task<IActionResult> GetOrdersByUserPhoneNumber(string phoneNumber)
        {
            try
            {
                var orders = await _orderRepo.GetOrdersByUserPhoneNumberAsync(phoneNumber);
                if (!orders.Any())
                {
                    return NotFound(new { Message = $"Orders with phoneNumber {phoneNumber} not found." });
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("summary")]
        public async Task<IActionResult> GetOrderSummary(DateTime startDate, DateTime endDate)
        {
            try
            {
                var summary = await _orderRepo.GetOrderSummaryAsync(startDate, endDate);
                if(summary == null)
                {
                    return NotFound(new { Message = $"No summary with these dates." });
                }
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("withItems")]
        public async Task<IActionResult> GetOrdersWithItems()
        {
            try
            {
                var orders = await _orderRepo.GetOrdersWithItemsAsync();
                if (!orders.Any())
                {
                    return NotFound(new { Message = $"Orders not found." });
                }
                return Ok(orders);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("recent/{days}")]
        public async Task<IActionResult> GetRecentOrders(int days)
        {
            try
            {
                var orders = await _orderRepo.GetRecentOrdersAsync(days);
                if (!orders.Any())
                {
                    return NotFound(new { Message = $"Orders within These {days} not found." });
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("topProducts/{topN}")]
        public async Task<IActionResult> GetTopSellingProducts(int topN)
        {
            try
            {
                var products = await _orderRepo.GetTopSellingProductsAsync(topN);
                if (!products.Any())
                {
                    return NotFound(new { Message = $"Products within These {topN} not found." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("totalSales")]
        public async Task<IActionResult> GetTotalSales()
        {
            try
            {
                var totalSales = await _orderRepo.GetTotalSalesAsync();
                return Ok(totalSales);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost()]
        public async Task<IActionResult> CreateOrder(CreateOrderDto orderDto)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var result = await _orderRepo.CreateOrderAsync(orderDto);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(new { result.Message });
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(string orderId, UpdateOrderDto orderDto)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var result = await _orderRepo.UpdateOrderAsync(orderId, orderDto);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(new { result.Message });
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("updateOrderItems/{orderId}")]
        public async Task<IActionResult> UpdateOrderItems(string orderId, [FromBody] List<UpdateOrderItem> updatedItems)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var result = await _orderRepo.UpdateOrderItemsAsync(orderId, updatedItems);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(result.Message);
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(result.Message);
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] OrderStatus status)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var result = await _orderRepo.UpdateOrderStatusAsync(orderId, status);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(new { result.Message });
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var result = await _orderRepo.DeleteOrderAsync(orderId);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(new { result.Message });
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                var result = await _orderRepo.CancelOrderAsync(orderId);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(new { result.Message });
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(new { result.Message });
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }
    }
}
