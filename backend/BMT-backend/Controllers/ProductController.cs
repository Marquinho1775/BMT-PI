using BMT_backend.Models;
using BMT_backend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase {

        private readonly ProductHandler _productHandler;

        public ProductController()
        {
            _productHandler = new ProductHandler();
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateProduct(ProductModel product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("Product information is not valid.");
                }
                var result = _productHandler.CreateProduct(product);
                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the product");
            }
        }

        [HttpGet]
        public List<ProductViewModel> GetProducts()
        {
            var products = _productHandler.GetProducts();
            return products;
        }

        [HttpGet("get-tags")]
        public List<string> GetTags()
        {
            var tags = _productHandler.GetTags();
            return tags;
        }

        [HttpGet("{enterpriseName}")]
        public ActionResult<List<ProductViewModel>> GetProductsByEnterprise(string enterpriseName)
        {
            try
            {
                var products = _productHandler.GetProductsByEnterprise(enterpriseName);
                return Ok(products);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los productos de la empresa.");
            }
        }


        [HttpPut("inventory")]
        public async Task<ActionResult<string>> UpdateStock(string id, int newStock){
            try
            {
                var result = _productHandler.UpdateStock(id, newStock);
                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the stock");
            }
        }
        [HttpPost("updateDateDisponibility")]
        public ActionResult<string> UpdateDateDisponibility(string PerishableProductId, string Date, int Quantity)
        {
            try
            {
                if (PerishableProductId == null || Date == null || Quantity == 0)
                {
                    return BadRequest("Product information is not valid.");
                }
                var result = _productHandler.UpdateDateDisponibility(PerishableProductId, Date, Quantity);
                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the product");
            }
        }
    }
}
