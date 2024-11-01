using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Models;
using BMT_backend.Handlers;
using BMT_backend.Infrastructure;
using System.Reflection.Metadata;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperController : Controller
    {
        private UserHandler _userHandler;
        private EnterpriseHandler _enterpriseHandler;
        private ProductHandler _productHandler;
        private OrderHandler _orderHandler;
        private MailManager _mailManager;
        public DeveloperController(IConfiguration configuration)
        {
            _userHandler = new UserHandler();
            _enterpriseHandler = new EnterpriseHandler();
            _productHandler = new ProductHandler();
            _orderHandler = new OrderHandler();
            _mailManager = new MailManager(configuration);
        }
        [HttpGet("getEnterprises")]
        public List<DevEnterpriseModel> GetEnterprises()
        {
            List<DevEnterpriseModel> devEnterprises = _enterpriseHandler.GetDevEnterprises();
            return devEnterprises;
        }
        [HttpGet("getProducts")]
        public List<DevProductModel> GetProducts()
        {
            List<DevProductModel> devProducts = _productHandler.GetDevProducts();
            return devProducts;
        }
        [HttpGet("getUsers")]
        public List<DevUserModel> GetUsers()
        {
            List<DevUserModel> devUsers = _userHandler.GetDevUsers();
            return devUsers;
        }
        [HttpGet("getToConfirmOrders")]
        public List<OrderModel> GetToConfirmOrders()
        {
            List<OrderModel> toConfirmOrders = _orderHandler.GetToConfirmOrders();
            return toConfirmOrders;
        }
        [HttpPut("ConfirmOrder")]
        public IActionResult ConfirmOrder(String orderID)
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
        public IActionResult DenyOrder(String orderID)
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
