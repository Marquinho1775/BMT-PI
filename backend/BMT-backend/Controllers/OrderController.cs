using Microsoft.AspNetCore.Mvc;
using BMT_backend.Models;
using BMT_backend.Handlers;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
        public IActionResult CreateOrder([FromBody] ShoppingCart cart, [FromQuery] string userId, [FromQuery] string userName, [FromBody] DirectionModel address)
        {
            if (cart == null || address == null)
            {
                return BadRequest("Invalid order data.");
            }

            string orderId = _orderHandler.CreateOrder(cart, userId, userName, address);

            if (string.IsNullOrEmpty(orderId))
            {
                return StatusCode(500, "Error creating the order.");
            }

            return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, null);
        }

        [HttpGet]
        public ActionResult<IEnumerable<OrderModel>> GetOrders()
        {
            var orders = _orderHandler.GetOrders();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public ActionResult<OrderModel> GetOrderById(int id)
        {
            var order = _orderHandler.GetOrderById(id);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            return Ok(order);
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateOrderStatus(int id, [FromBody] int newStatus)
        {
            bool result = _orderHandler.UpdateOrderStatus(id.ToString(), newStatus);

            if (!result)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            return NoContent(); // Indica que la actualización fue exitosa
        }
    }
}