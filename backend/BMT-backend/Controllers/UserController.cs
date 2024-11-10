using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BMT_backend.Handlers;
using BMT_backend.Infrastructure;
using BMT_backend.Domain.Requests;
using BMT_backend.Domain.Entities;
using BMT_backend.Application.Services;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private UserHandler _userHandler;
        private OrderHandler _orderHandler;
        private MailManager _mailManager;
        private readonly TokenService _tokenService;

        public UserController(TokenService tokenService, IConfiguration configuration)
        {
            _userHandler = new UserHandler();
            _orderHandler = new OrderHandler();
            _mailManager = new MailManager(configuration);
            _tokenService = tokenService;
        }

        [HttpGet]
        public List<User> GetUsers()
        {
            try
            {
                return _userHandler.GetUsers();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving users: {ex.Message}");
                Response.StatusCode = 500;
                return new List<User>();
            }
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateUser(User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest();
                }

                var result = _userHandler.CreateUser(user);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creando al usuario");
            }
        }

        [HttpPost("Role")]
        public async Task<ActionResult<bool>> UpdateRole(string id, string role)
        {
            try
            {
                var result = _userHandler.UpdateRole(id, role);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error actualizando el rol del usuario");
            }
        }

        [HttpGet("GetUserByUsername")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                var user = _userHandler.GetUserByUsername(username);

                if (user == null)
                {
                    return NotFound("Usuario no encontrado");
                }

                return new JsonResult(user);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener el usuario");
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginData)
        {
            var existingUser = _userHandler.GetUsers()
                .FirstOrDefault(u => u.Email == loginData.Email && u.Password == loginData.Password);

            if (existingUser != null)
            {
                var token = _tokenService.GenerateToken(existingUser);
                return Ok(new { Token = token, User = existingUser });
            }

            return Unauthorized("Invalid email or password");
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest updatedUser)
        {
            try
            {
                if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Id))
                {
                    return BadRequest("Datos de usuario inválidos");
                }

                var result = _userHandler.UpdateUserProfile(updatedUser);

                if (result)
                {
                    return Ok("Perfil actualizado correctamente");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo actualizar el perfil");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el perfil: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el perfil");
            }
        }
        [HttpGet("GetToConfirmOrders")]
        public List<Order> GetToConfirmOrders(string userId)
        {
            List<Order> toConfirmOrders = _orderHandler.GetToConfirmUserOrders(userId);
            return toConfirmOrders;
        }
        [HttpPut("DenyOrder")]
        public IActionResult DenyOrder(string orderID)
        {
            if (_orderHandler.DenyOrder(orderID))
            {
                try
                {
                    var order = _orderHandler.GetOrderById(orderID);
                    if (order == null)
                    {
                        return NotFound($"Order with ID {orderID} not found.");
                    }

                    _mailManager.SendDenyEmail(order);
                    return Ok("Order cancelled, email sent.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error sending cancelled notification email.");
                }
            }
            return BadRequest("Order cancellation failed.");
        }
    }
}
