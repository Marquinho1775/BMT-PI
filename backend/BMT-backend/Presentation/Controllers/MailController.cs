using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BMT_backend.Handlers;
using BMT_backend.Domain.Entities;
using BMT_backend.Application.Services;
using BMT_backend.Infrastructure;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly UserService _userService;

        private readonly CodeHandler _codeHandler;
        private readonly MailManager _mailManager;

        public EmailController(IConfiguration configuration, UserService userService)
        {
            _userService = userService;
            _codeHandler = new CodeHandler();
            _mailManager = new MailManager(configuration);
        }

        [HttpPost("sendemail")]
        public IActionResult SendMail(Mail userData)
        {
            try
            {
                _mailManager.SendVerificationEmail(userData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar correo: {ex.Message}");
            }
        }

        [HttpPost("verifycode")]
        public IActionResult VerifyCode(ConfirmationCode codeModel)
        {
            try
            {
                string realCode = _codeHandler.GetCode(codeModel.Id);
                if (realCode == codeModel.Code)
                {
                    return Ok();
                }
                else
                {
                    return Unauthorized("Código de verificación incorrecto.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al verificar código: {ex.Message}");
            }
        }

        [HttpPost("verifyaccount")]
        public IActionResult VerifyAccount(ConfirmationCode account)
        {
            try
            {
                _userService.UpdateAccountVerification(account.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al verificar cuenta: {ex.Message}");
            }
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
