using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Models;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly string email;
        private readonly string host;
        private readonly int port;
        private readonly string password;
        public EmailController(IConfiguration configuration)
        {
            email = configuration["EmailSettings:Email"];
            host = configuration["EmailSettings:Host"];
            port = int.Parse(configuration["EmailSettings:Port"]);
            password = configuration["EmailSettings:Password"];
        }
        [HttpPost("sendemail")]
        public IActionResult SendMail(string receptor)
        {
            try
            {
                string title = "Verificación de correo Business Tracker";
                string body = "<h1>Verifica tu correo</h1>";
                body += "Hola!<br><br>Gracias por registrarte en Business Tracker.<br>";
                body += "Para verificar tu cuenta, necesitamos que verifiques tu dirección de correo electrónico.<br><br>";
                body += "<b>[Código]</b><br><br>";
                body += "Si no has creado una cuenta en Business Tracker, puedes ignorar este correo.<br><br>";
                body += "Si tienes alguna pregunta, no dudes en contactarnos.<br><br>";
                body += "¡Bienvenido a bordo!<br><br>";
                body += "Saludos,<br>";
                body += "El equipo de Business Tracker";

                // Creando el correo
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = title,
                    Body = body,
                };
                mailMessage.IsBodyHtml = true;

                // Seleccionando el destinatario
                mailMessage.To.Add(receptor);

                // Setteando la información necesaria para enviar el correo
                using var smtpClient = new SmtpClient();
                smtpClient.Host = host;
                smtpClient.Port = port;
                smtpClient.Credentials = new NetworkCredential(email, password);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
