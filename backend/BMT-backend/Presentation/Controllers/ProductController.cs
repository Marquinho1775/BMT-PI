﻿//using Microsoft.AspNetCore.Mvc;
//using BMT_backend.Handlers;
//using BMT_backend.Infrastructure;
//using BMT_backend.Domain.Entities;
//using BMT_backend.Application.Services;
//using BMT_backend.Presentation.DTOs;
//using BMT_backend.Presentation.Requests;

//namespace BMT_backend.Presentation.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductController : ControllerBase
//    {

//        private readonly ProductService productService;

//        private readonly ProductHandler _productHandler;

//        public ProductController(IConfiguration configuration)
//        {
//            _productHandler = new ProductHandler(configuration);
//        }

//        [HttpPost]
//        public async Task<ActionResult<string>> CreateProduct(Product product)
//        {
//            try
//            {
//                if (product == null)
//                {
//                    return BadRequest("Product information is not valid.");
//                }
//                var result = _productHandler.CreateProduct(product);
//                return result;
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the product");
//            }
//        }

//        [HttpGet]
//        public List<Product> GetProducts()
//        {
//            var products = _productHandler.GetProducts();
//            return products;
//        }

//        //YEPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
//        [HttpGet("{enterpriseName}")]
//        public ActionResult<List<Product>> GetProductsByEnterprise(string enterpriseName)
//        {
//            try
//            {
//                var products = _productHandler.GetProductsByEnterprise(enterpriseName);
//                return Ok(products);
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los productos de la empresa.");
//            }
//        }

//        [HttpPost("get-stock")]
//        public int GetStock(GetProductStockRequest product)
//        {
//            int cant = 0;
//            try
//            {
//                cant = _productHandler.GetStock(product);
//                return cant;
//            }
//            catch (Exception)
//            {
//                return cant;
//            }
//        }

//        [HttpPut]
//        public ActionResult UpdateProduct([FromBody] Product updatedProduct)
//        {
//            try
//            {
//                var updated = _productHandler.UpdateProduct(updatedProduct);

//                if (!updated)
//                    return NotFound("Producto no encontrado o no se pudo actualizar.");

//                return Ok("Producto actualizado exitosamente.");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el producto: " + ex.Message);
//            }
//        }

//        [HttpPut("UpdateStock")]
//        public ActionResult<string> UpdateStock(UpdateProductStockRequest product)
//        {
//            try
//            {
//                if (ProductId == null || Date == null || Quantity == 0)
//                {
//                    return BadRequest("Product information is not valid.");
//                }
//                var result = _productHandler.UpdateStock(ProductId, Quantity, Date);
//                return result;
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the product");
//            }
//        }
//    }
//}
