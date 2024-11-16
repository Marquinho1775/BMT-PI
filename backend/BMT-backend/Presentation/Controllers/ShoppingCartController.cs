//using BMT_backend.Domain.Entities;
//using BMT_backend.Handlers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using BMT_backend.Application.Services;

//namespace BMT_backend.Presentation.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ShoppingCartController : ControllerBase
//    {
//        private readonly ShoppingCartService _shoppingCartService;

//        public ShoppingCartController(IConfiguration configuration, ShoppingCartService shoppingCartService)
//        {
//            _shoppingCartService = shoppingCartService;
//        }

//        [HttpPost]
//        public async Task<ActionResult<string>> CreateShoppingCartAsync(string userName)
//        {
//            try
//            {
//                if (userName == null)
//                {
//                    return BadRequest("User Id is not valid.");
//                }
//                var result = _shoppingCartService.CreateShoppingCartAsync(userName);
//                return result ? Ok("Shopping cart created successfully.") : StatusCode(StatusCodes.Status500InternalServerError, "Error creating the shopping cart.");
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the shopping cart");
//            }
//        }
//        [HttpGet]
//        public ActionResult<ShoppingCart> GetShoppingCartAsync(string userId)
//        {
//            try
//            {
//                var shoppingCart = _shoppingCartService.GetShoppingCartAsync(userId);
//                return Ok(shoppingCart);
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting the shopping cart");
//            }
//        }
//        [HttpGet("GetCartId")]
//        public ActionResult<string> GetCartIdAsync(string userId)
//        {
//            try
//            {
//                var shoppingCartId = _shoppingCartService.GetCartIdAsync(userId);
//                return Ok(shoppingCartId);
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting the shopping cart");
//            }
//        }
//        [HttpPut("AddProductToCart")]
//        public ActionResult<bool> AddProductToCartAsync(string shoppingCartId, string productId)
//        {
//            try
//            {
//                var result = _shoppingCartService.AddProductToCartAsync(shoppingCartId, productId);
//                return new JsonResult(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
//            }
//        }
//        [HttpPut("ChangeProductQuantity")]
//        public ActionResult<bool> ChangeProductQuantityAsync(string shoppingCartId, string productId, int quantity)
//        {
//            try
//            {
//                var result = _shoppingCartService.ChangeProductQuantityAsync(shoppingCartId, productId, quantity);
//                return new JsonResult(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
//            }
//        }
//        [HttpDelete("DeleteProductFromCart")]
//        public ActionResult<bool> DeleteProductFromCartAsync(string shoppingCartId, string productId)
//        {
//            try
//            {
//                var result = _shoppingCartService.DeleteProductFromCartAsync(shoppingCartId, productId);
//                return new JsonResult(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
//            }
//        }
//        [HttpDelete("ClearShoppingCart")]
//        public ActionResult<bool> ClearShoppingCartAsync(string shoppingCartId)
//        {
//            try
//            {
//                var result = _shoppingCartService.ClearShoppingCartAsync(shoppingCartId);
//                return new JsonResult(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
//            }
//        }
//    }
//}