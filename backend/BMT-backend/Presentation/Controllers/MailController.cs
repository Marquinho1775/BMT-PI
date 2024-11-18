using Microsoft.AspNetCore.Mvc;
using BMT_backend.Domain.Entities;
using BMT_backend.Application.Services;
using BMT_backend.Application.Interfaces;
using BMT_backend.Infrastructure;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly MailService _mailService;

        public EmailController(IConfiguration configuration, UserService userService, MailService mailService)
        {
            _userService = userService;
            _mailService = mailService;
        }

        [HttpPost("SendEmail")]
        public IActionResult SendMail(Mail userData)
        {
            try
            {
                _mailService.SendVerificationEmail(userData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar correo: {ex.Message}");
            }
        }

        [HttpPost("SendCollabMail")]
        public IActionResult SendCollabMail([FromBody] CollabMail userData)
        {
            try
            {
                _mailService.SendCollabInviteEmail(userData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar correo de colaboración: {ex.Message}");
            }
        }

        [HttpPost("SendConfirmedCollabMail")]
        public IActionResult ConfirmedCollabMail([FromBody] Mail userData)
        {
            try
            {
                _mailService.SendConfirmedCollabEmail(userData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar correo de confirmación: {ex.Message}");
            }
        }
    }
}
