using Microsoft.AspNetCore.Mvc;
using BMT_backend.Infrastructure;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Application.Services;
using BMT_backend.Application.Interfaces;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperController : Controller
    {
        private ProductService _productService;
        private readonly UserService _userService;
        private readonly EnterpriseService _enterpriseService;
        private readonly ICodeRepository _codeRepository;
        private readonly MailService _mailManager;
        private readonly OrderService _orderService;

        public DeveloperController(UserService userService, IConfiguration configuration, ProductService productService, ICodeRepository codeRepository, EnterpriseService enterpriseService, OrderService orderService)
        {
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _mailManager = new MailService(configuration, codeRepository);
            _enterpriseService = enterpriseService;
        }

        [HttpGet("getEnterprises")]
        public async Task<List<EnterpriseDevDto>> GetEnterprises()
        {
            List<EnterpriseDevDto> devEnterprises = await _enterpriseService.GetAllEnterpriseDevAsnc();
            return devEnterprises;
        }

        [HttpGet("getProducts")]
        public async Task<List<Product>> GetProducts()
        {
            List<Product> devProducts = await _productService.GetProductsAsync();
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
        public async Task<List<OrderDetails>> GetToConfirmOrders()
        {
            List<OrderDetails> toConfirmOrders = await _orderService.GetToConfirmOrders();
            return toConfirmOrders;
        }

        [HttpPut("ConfirmOrder")]
        public async Task<IActionResult> ConfirmOrder(string orderID)
        {
            if (await _orderService.ConfirmOrder(orderID))
            {
                try
                {
                    var order = await _orderService.GetOrderDetailsById(orderID);
                    if (order == null)
                    {
                        return NotFound($"Order with ID {orderID} not found.");
                    }
                    _mailManager.SendConfirmationEmails(order);
                    return Ok("Order confirmed and emails sent.");
                }
                catch (Exception)
                {
                    return StatusCode(500, "Error sending confirmation emails.");
                }
            }
            return BadRequest("Order confirmation failed.");
        }

        [HttpPut("DenyOrder")]
        public async Task<IActionResult> DenyOrder(string orderID)
        {
            if (await _orderService.DenyOrder(orderID))
            {
                try
                {
                    var order = await _orderService.GetOrderDetailsById(orderID);
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
