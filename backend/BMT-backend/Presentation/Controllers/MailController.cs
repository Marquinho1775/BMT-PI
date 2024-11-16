using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BMT_backend.Handlers;
using BMT_backend.Domain.Entities;
using BMT_backend.Application.Services;
using BMT_backend.Infrastructure;
using BMT_backend.Infrastructure.Data;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly MailManager _mailManager;

        public EmailController(IConfiguration configuration, UserService userService, CodeRepository codeRepository)
        {
            _userService = userService;
            _mailManager = new MailManager(configuration, codeRepository);
        }



        [HttpPost("sendcollabmail")]
        public IActionResult SendCollabMail([FromBody] CollabMail userData)
        {
            try
            {
                _mailManager.SendCollabInviteEmail(userData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar correo de colaboración: {ex.Message}");
            }
        }

        [HttpPost("sendconfirmedcollabmail")]
        public IActionResult ConfirmedCollabMail([FromBody] Mail userData)
        {
            try
            {
                _mailManager.SendConfirmedCollabEmail(userData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar correo de confirmación: {ex.Message}");
            }
        }
    }
}
