//using BMT_backend.Domain.Entities;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using BMT_backend.Application.Services;
//using BMT_backend.Infrastructure;

//namespace BMT_backend.Presentation.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ShoppingCartController : ControllerBase
//    {
//        private readonly ShoppingCartService _shopingCartServices;

//        public ShoppingCartController(IConfiguration configuration, ShoppingCartService shoppingCartService)
//        {
//            _shopingCartServices = shoppingCartService;
//        }

//        [HttpPost]
//        public async Task<ActionResult<string>> CreateShoppingCart(string userName)
//        {
//            try
//            {
//                if (userName == null)
//                {
//                    return BadRequest("User Id is not valid.");
//                }
//                var result = _shopingCartServices.CreateShoppingCartAsync(userName);
//                return await result ? Ok("Shopping cart created successfully.") : StatusCode(StatusCodes.Status500InternalServerError, "Error creating the shopping cart.");
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the shopping cart");
//            }
//        }
//        [HttpGet]
//        public ActionResult<ShoppingCart> GetShoppingCart(string userId)
//        {
//            try
//            {
//                var shoppingCart = _shopingCartServices.GetShoppingCartByUserIdAsync(userId);
//                return Ok(shoppingCart);
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting the shopping cart");
//            }
//        }

//        [HttpGet("GetCartId")]
//        public ActionResult<string> GetCartId(string userId)
//        {
//            try
//            {
//                var shoppingCartId = _shopingCartServices.GetCartIdAsync(userId);
//                return Ok(shoppingCartId);
//            }
//            catch (Exception)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting the shopping cart");
//            }
//        }
//        [HttpPut("AddProductToCart")]
//        public ActionResult<bool> AddProductToCart(string shoppingCartId, string productId)
//        {
//            try
//            {
//                var result = _shopingCartServices.AddProductToCartAsync(shoppingCartId, productId);
//                return new JsonResult(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
//            }
//        }
//        [HttpPut("ChangeProductQuantity")]
//        public ActionResult<bool> ChangeProductQuantity(string shoppingCartId, string productId, int quantity)
//        {
//            try
//            {
//                var result = _shopingCartServices.ChangeProductQuantityAsync(shoppingCartId, productId, quantity);
//                return new JsonResult(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
//            }
//        }
//        [HttpDelete("DeleteProductFromCart")]
//        public ActionResult<bool> DeleteProductFromCart(string shoppingCartId, string productId)
//        {
//            try
//            {
//                var result = _shopingCartServices.RemoveProductFromCartAsync(shoppingCartId, productId);
//                return new JsonResult(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
//            }
//        }
//        [HttpDelete("ClearShoppingCart")]
//        public ActionResult<bool> ClearShoppingCart(string shoppingCartId)
//        {
//            try
//            {
//                var result = _shopingCartServices.ClearShoppingCartAsync(shoppingCartId);
//                return new JsonResult(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, "Error agregando producto al carrito");
//            }
//        }
//    }
//}
