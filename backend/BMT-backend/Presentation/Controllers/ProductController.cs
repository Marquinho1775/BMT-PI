using Microsoft.AspNetCore.Mvc;
using BMT_backend.Application.Services;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.Requests;
using System;
using System.Threading.Tasks;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] Product product)
        {
            if (product == null)
                return BadRequest(new { Message = "La información del producto no es válida." });
            try
            {
                var result = await _productService.CreateProduct(product);
                if (result)
                    return Ok(new { Success = true, Message = "Producto creado exitosamente." });
                else
                    return StatusCode(500, new { Success = false, Message = "No se pudo crear el producto." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productService.GetProductsAsync();
                return Ok(new { Success = true, Data = products });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor." });
            }
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { Message = "El id del producto no es válido." });
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product != null)
                    return Ok(new { Success = true, Data = product });
                else
                    return NotFound(new { Success = false, Message = "Producto no encontrado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetProductsDetails")]
        public async Task<IActionResult> GetProductsDetails()
        {
            try
            {
                var products = await _productService.GetProducsDetailsAsync();
                return Ok(new { Success = true, Data = products });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor." });
            }
        }

        [HttpGet("GetProductDetailsById")]
        public async Task<IActionResult> GetProductDetailsById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { Message = "El id del producto no es válido." });
            try
            {
                var product = await _productService.GetProductDetailsByIdAsync(id);
                if (product != null)
                    return Ok(new { Success = true, Data = product });
                else
                    return NotFound(new { Success = false, Message = "Producto no encontrado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("get-stock")]
        public async Task<IActionResult> GetStock(GetProductStockRequest product)
        {
            int cant = 0;
            try
            {
                cant = await _productService.GetStock(product);
                return Ok(new { Success = true, Stock = cant });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromForm] Product updatedProduct)
        {
            if (updatedProduct == null || string.IsNullOrEmpty(updatedProduct.Id))
                return BadRequest(new { Message = "La información del producto no es válida." });
            try
            {
                var result = await _productService.UpdateProductAsync(updatedProduct);
                if (result)
                    return Ok(new { Success = true, Message = "Producto actualizado exitosamente." });
                else
                    return NotFound(new { Success = false, Message = "Producto no encontrado o no se pudo actualizar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error al actualizar el producto: " + ex.Message });
            }
        }

        [HttpPut("UpdateStock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateProductStockRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ProductId))
                return BadRequest(new { Message = "La información del producto no es válida." });
            try
            {
                var result = await _productService.UpdateStockAsync(request.ProductId, request.Quantity, request.Type, request.DateString);

                if (result)
                    return Ok(new { Success = true, Message = "Stock actualizado exitosamente." });
                else
                    return NotFound(new { Success = false, Message = "Producto no encontrado o no se pudo actualizar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error al actualizar el stock del producto: " + ex.Message });
            }
        }
    }
}
