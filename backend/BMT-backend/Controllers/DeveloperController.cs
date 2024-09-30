using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Models;
using BMT_backend.Handlers;
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
        public DeveloperController() {
            _userHandler = new UserHandler();
            _enterpriseHandler = new EnterpriseHandler();
            _productHandler = new ProductHandler();
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
    }
}
