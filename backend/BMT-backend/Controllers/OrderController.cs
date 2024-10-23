using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Models;
using BMT_backend.Handlers;
using Microsoft.Extensions.Configuration;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderHandler _orderHandler;

        public OrderController(IConfiguration configuration)
        {
            _orderHandler = new OrderHandler(configuration);
        }

        [HttpPost]
        public IActionResult CreateOrder(OrderModel order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest("Order information is not valid.");
                }
                var orderId = _orderHandler.CreateOrder(order);
                return Ok(new { Id = orderId });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the order.");
            }
        }

        [HttpGet]
        public IEnumerable<OrderModel> GetOrders()
        {
            return _orderHandler.GetOrders();
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateOrderStatus(string id, int newStatus)
        {
            try
            {
                var result = _orderHandler.UpdateOrderStatus(id, newStatus);
                if (!result)
                {
                    return NotFound("Order not found.");
                }
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the order status.");
            }
        }

        [HttpGet("orders-with-products")]
        public IActionResult GetOrdersWithProducts()
        {
            try
            {
                var orders = _orderHandler.GetOrders();
                return Ok(orders);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving orders.");
            }
        }
    }
}