using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BMT_backend.Handlers;
using BMT_backend.Models;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private UserHandler _userHandler;
        private readonly TokenService _tokenService;

        public UserController(TokenService tokenService)
        {
            _userHandler = new UserHandler();
            _tokenService = tokenService;
        }

        [HttpGet]
        public List<UserModel> GetUsers()
        {
            try
            {
                return _userHandler.GetUsers();
            }
            catch (Exception ex)
            {
                // Log the exception message (you may replace this with proper logging)
                Console.WriteLine($"Error retrieving users: {ex.Message}");
                // Optionally, you can return a 500 status code with a detailed message
                Response.StatusCode = 500;
                return new List<UserModel>();
            }
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateUser(UserModel user)
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginData)
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
    }
}
