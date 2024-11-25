using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Application.Services;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(IConfiguration configuration, OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateOrder(Order order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest();
                }
                var result = await _orderService.CreateOrder(order);
                return Ok(new { id = result });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creando la orden");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDetails>>> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetOrdersDetails();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo las ordenes");
            }
        }

        [HttpPost("AddProductToOrder")]
        public async Task<ActionResult<string>> AddProductToOrder(AddProductToOrderRequest orderProduct)
        {
            try
            {
                if (orderProduct == null)
                {
                    return BadRequest();
                }
                var result = await _orderService.AddProductToOrder(orderProduct);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error añadiendo producto a la orden");
            }
        }

        [HttpPost("OrderReports")]
        public async Task<IActionResult> GetOrderReports(ReportRequest report)
        {
            try
            {
                var orders = await _orderService.GetOrderReportsAsync(report);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo las ordenes");
            }
        }

        [HttpGet("IsDirectionUsedInOrders")]
        public async Task<ActionResult<bool>> IsDirectionUsedInOrders(string directionId)
        {
            try
            {
                var result = await _orderService.IsDirectionUsedInOrders(directionId);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error verificando si la dirección está en uso");
            }
        }

        [HttpGet("isProductUsedInOrdersAsync")]
        public async Task<ActionResult<bool>> IsProductUsedInOrdersAsync(string productId)
        {
            try
            {
                var result = await _orderService.IsProductUsedInOrders(productId);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error verificando si el producto está en uso");
            }
        }

        [HttpGet("AreEnterpriseProductsInOrders")]
        public async Task<ActionResult<bool>> AreEnterpriseProductsInOrders(string enterpriseId)
        {
            try
            {
                var result = await _orderService.AreEnterpriseProductsInOrders(enterpriseId);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error verificando si el producto de la empresa está en la orden");
            }
        }
    }
}
