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
        [HttpPut("UpdateStock")]
        public ActionResult<string> UpdateStock(string ProductId, string Date, int Quantity)
        {
            try
            {
                if (ProductId == null || Date == null || Quantity == 0)
                {
                    return BadRequest("Product information is not valid.");
                }
                var result = _productHandler.UpdateStock(ProductId, Date, Quantity);
                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the product");
            }
        }

        [HttpPost("get-stock")]
        public int GetStock(CheckOutProductModel product)
        {
            int cant = 0;
            if (product.Type == "NonPerishable")
            {
                try
                {
                    cant = _productHandler.GetStock(product.ProductId);
                }
                catch (Exception)
                {
                    return cant;
                }
                return cant;
            }

            if (product.Type == "Perishable")
            {
                try
                {
                    cant = _productHandler.GetStockPerishable(product.ProductId, product.Date);
                }
                catch (Exception)
                {
                    return cant;
                }
                return cant;
            }
            return cant;
        }

        [HttpPut]
        public ActionResult UpdateProduct([FromBody] ProductModel updatedProduct)
        {
            try
            {
                var updated = _productHandler.UpdateProduct(updatedProduct);

                if (!updated)
                    return NotFound("Producto no encontrado o no se pudo actualizar.");

                return Ok("Producto actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el producto: " + ex.Message);
            }
        }

        [HttpGet("get-tags-by-product/{productId}")]
        public ActionResult<List<string>> GetTagsByProductId(string productId)
        {
            try
            {
                var tags = _productHandler.GetTagsIDBasedOnProductID(productId);

                if (tags == null || tags.Count == 0)
                    return NotFound("No se encontraron tags asociados a este producto.");

                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los tags: " + ex.Message);
            }
        }

        [HttpGet("get-tags-by-name/{tagName}")]
        public ActionResult<List<string>> GetTagsByName(string tagName)
        {
            try
            {
                var tags = _productHandler.GetTagsIDBasedOnTagName(tagName);

                if (tags == null || tags.Count == 0)
                    return NotFound("No se encontraron tags con ese nombre.");

                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los IDs de los tags: " + ex.Message);
            }
        }


    }
}
