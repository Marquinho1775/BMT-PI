using BMT_backend.Handlers;
using BMT_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderHandler _orderHandler;
        public OrderController()
        {
            _orderHandler = new OrderHandler();
        }

        [HttpGet]
        public List<OrderModel> GetOrders()
        {
            try
            {
                var orders = _orderHandler.GetOrders();
                return orders;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        public async Task<ActionResult<string>> CreateOrder(OrderModel order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest();
                }
                var result = _orderHandler.CreateOrder(order);
                return Ok(new { id = result });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creando la orden");
            }
        }
        [HttpPost("AddProductToOrder")]
        public async Task<ActionResult<string>> AddProductToOrder(OrderProductModel orderProduct)
        {
            try
            {
                if (orderProduct == null)
                {
                    return BadRequest();
                }
                var result = _orderHandler.AddProductToOrder(orderProduct);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error añadiendo producto a la orden");
            }
        }
        [HttpPut("UpdateDeliverFee")]
        public async Task<ActionResult<string>> UpdateDeliverFee(string orderId)
        {
            try
            {
                if (orderId == null)
                {
                    return BadRequest();
                }
                var result = _orderHandler.UpdateDeliverFee(orderId);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error actualizando la tarifa de envío");
            }
        }
    }
}
