using Microsoft.AspNetCore.Mvc;
using BMT_backend.Handlers;
using BMT_backend.Infrastructure;
using BMT_backend.Domain.Views;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Application.Services;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperController : Controller
    {
        private readonly UserService _userService;

        private EnterpriseHandler _enterpriseHandler;
        private ProductHandler _productHandler;
        private OrderHandler _orderHandler;
        private MailManager _mailManager;
        public DeveloperController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;

            _enterpriseHandler = new EnterpriseHandler();
            _productHandler = new ProductHandler(configuration);
            _orderHandler = new OrderHandler(configuration);
            _mailManager = new MailManager(configuration);
        }

        [HttpGet("getEnterprises")]
        public List<DeveloperEnterpriseView> GetEnterprises()
        {
            List<DeveloperEnterpriseView> devEnterprises = _enterpriseHandler.GetDevEnterprises();
            return devEnterprises;
        }

        [HttpGet("getProducts")]
        public List<Product> GetProducts()
        {
            List<Product> devProducts = _productHandler.GetProducts();
            return devProducts;
        }

        [HttpGet("getUsers")]
        public async Task<List<UserDevDto>> GetUsers()
        {
            List<UserDevDto> devUsers = await _userService.GetAllUserDevAsync();
            Console.WriteLine(devUsers);
            return devUsers;
        }

        [HttpGet("getToConfirmOrders")]
        public List<OrderDetails> GetToConfirmOrders()
        {
            List<OrderDetails> toConfirmOrders = _orderHandler.GetToConfirmOrders();
            return toConfirmOrders;
        }

        [HttpPut("ConfirmOrder")]
        public IActionResult ConfirmOrder(string orderID)
        {
            if (_orderHandler.ConfirmOrder(orderID))
            {
                try
                {
                    var order = _orderHandler.GetOrderById(orderID);
                    if (order == null)
                    {
                        return NotFound($"Order with ID {orderID} not found.");
                    }

                    _mailManager.SendConfirmationEmails(order);
                    return Ok("Order confirmed and emails sent.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error sending confirmation emails.");
                }
            }
            return BadRequest("Order confirmation failed.");
        }

        [HttpPut("DenyOrder")]
        public IActionResult DenyOrder(string orderID)
        {
            if (_orderHandler.DenyOrder(orderID))
            {
                try
                {
                    var order = _orderHandler.GetOrderById(orderID);
                    Console.WriteLine(order);
                    if (order == null)
                    {
                        return NotFound($"Order with ID {orderID} not found.");
                    }

                    _mailManager.SendDenyEmail(order);
                    return Ok("Order cancelled, email sent.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error sending cancelled notification email.");
                }
            }
            return BadRequest("Order cancellation failed.");
        }
    }
}
