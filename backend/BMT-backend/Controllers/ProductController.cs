using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BMT_backend.Handlers;
using BMT_backend.Models;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private ProductHandler _productHandler;

        public ProductController()
        {
            _productHandler = new ProductHandler();
        }

        [HttpGet]
        public List<DevProductModel> GetDevProducts()
        {
            try
            {
                return _productHandler.GetDevProducts();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                Response.StatusCode = 500;
                return new List<DevProductModel>();
            }
        }
    }
}
