using BMT_backend.Models;
using BMT_backend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
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
                var productId = _productHandler.CreateProduct(product);
                if (string.IsNullOrEmpty(productId))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the product");
                }
                return Ok(productId);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the product");
            }
        }

        [HttpPost("non-perishable")]
        public async Task<ActionResult<bool>> CreateNonPerishableProduct(NonPerishableProductModel product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("Product information is not valid.");
                }
                var result = _productHandler.CreateNonPerishableProduct(product);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the product");
            }
        }

        [HttpPost("perishable")]
        public async Task<ActionResult<bool>> CreatePerishableProduct(PerishableProductModel product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("Product information is not valid.");
                }
                var result = _productHandler.CreatePerishableProduct(product);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the product");
            }
        }

        [HttpPost("add-tags")]
        public async Task<ActionResult<bool>> AddTagsToProduct(AddTagsToProductRequest request)
        [HttpGet]
        public List<DevProductModel> GetDevProducts()
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Product information is not valid.");
                }
                var result = _productHandler.AddTagsToProduct(request);
                return new JsonResult(result);
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
    }
}
