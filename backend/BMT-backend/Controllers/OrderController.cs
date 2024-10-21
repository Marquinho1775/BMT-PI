using BMT_backend.Handlers;
using BMT_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderHandler _orderHandler;

        public OrderController(OrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }

        [HttpPost]

        public IActionResult CreateOrder(Order order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = _orderHandler.CreateOrder(order);
                if (result)
                {
                    return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
                }
                else
                {
                    return StatusCode(500, "Error creating order");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var order = _orderHandler.GetOrderById(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}