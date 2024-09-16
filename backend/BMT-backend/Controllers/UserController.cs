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

        public UserController()
        {
            _userHandler = new UserHandler();
        }

        [HttpGet]
        public List<UserModel> GetUsers()
        {
            return _userHandler.GetUsers();
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

                UserHandler userHandler = new UserHandler();
                var result = userHandler.CreateUser(user);
                return new JsonResult(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creando al usuario");
            }
        }
    }
}
