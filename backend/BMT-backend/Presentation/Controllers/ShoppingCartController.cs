using BMT_backend.Domain.Entities;
using BMT_backend.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ShoppingCartHandler _shoppingCartHandler;

        public ShoppingCartController(IConfiguration configuration)
        {
            _shoppingCartHandler = new ShoppingCartHandler(configuration);
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateShoppingCart(string userName)
        {
            try
            {
                if (userName == null)
                {
                    return BadRequest("User Id is not valid.");
                }
                var result = _shoppingCartHandler.CreateShoppingCart(userName);
                return result ? Ok("Shopping cart created successfully.") : StatusCode(StatusCodes.Status500InternalServerError, "Error creating the shopping cart.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the shopping cart");
            }
        }
        [HttpGet]
        public ActionResult<ShoppingCart> GetShoppingCart(string userId)
        {
            try
            {
                var shoppingCart = _shoppingCartHandler.GetShoppingCart(userId);
                return Ok(shoppingCart);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting the shopping cart");
            }
        }
        [HttpGet("GetCartId")]
        public ActionResult<string> GetCartId(string userId)
        {
            try
            {
                var shoppingCartId = _shoppingCartHandler.GetCartId(userId);
                return Ok(shoppingCartId);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting the shopping cart");
            }
        }
        [HttpPut("AddProductToCart")]
        public ActionResult<bool> AddProductToCart(string shoppingCartId, string productId)
        {
            try
            {
                var result = _shoppingCartHandler.AddProductToCart(shoppingCartId, productId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
            }
        }
        [HttpPut("ChangeProductQuantity")]
        public ActionResult<bool> ChangeProductQuantity(string shoppingCartId, string productId, int quantity)
        {
            try
            {
                var result = _shoppingCartHandler.ChangeProductQuantity(shoppingCartId, productId, quantity);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
            }
        }
        [HttpDelete("DeleteProductFromCart")]
        public ActionResult<bool> DeleteProductFromCart(string shoppingCartId, string productId)
        {
            try
            {
                var result = _shoppingCartHandler.DeleteProductFromCart(shoppingCartId, productId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
            }
        }
        [HttpDelete("ClearShoppingCart")]
        public ActionResult<bool> ClearShoppingCart(string shoppingCartId)
        {
            try
            {
                var result = _shoppingCartHandler.ClearShoppingCart(shoppingCartId);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
            }
        }
    }
}
