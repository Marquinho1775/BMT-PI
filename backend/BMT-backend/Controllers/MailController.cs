﻿using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Models;
using System.Data.SqlClient;
using BMT_backend.Handlers;

namespace BMT_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private CodeHandler _codeHandler;
        private UserHandler _userHandler;
        private EntrepreneurHandler _entrepeneurHandler;
        private readonly string email;
        private readonly string emailCollab;
        private readonly string host;
        private readonly int port;
        private readonly string password;
        private readonly string passwordCollab;
        public EmailController(IConfiguration configuration)
        {
            _userHandler = new UserHandler();
            _codeHandler = new CodeHandler();
            email = configuration["EmailSettings:Email"];
            emailCollab = configuration["EmailSettingsCollabRegister:Email"];
            host = configuration["EmailSettings:Host"];
            port = int.Parse(configuration["EmailSettings:Port"]);
            password = configuration["EmailSettings:Password"];
            passwordCollab = configuration["EmailSettingsCollabRegister:Password"];
        }
        [HttpPost("sendemail")]
        public IActionResult SendMail(MailModel userData)
        {
            try
            {
                string code = _codeHandler.CreateCode(userData.Id);

                string title = "Verificación de correo Business Tracker";
                string body = "<h1>Verifica tu correo</h1>";
                body += "Hola!<br><br>Gracias por registrarte en Business Tracker.<br>";
                body += "Para verificar tu cuenta, necesitamos que verifiques tu dirección de correo electrónico.<br><br>";
                body += "<b>" + code + "</b><br><br>";
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
                mailMessage.To.Add(userData.Email);

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
        [HttpPost("verifycode")]
        public IActionResult VerifyCode(CodeModel codeModel)
        {
            try
            {
                string realCode = _codeHandler.GetCode(codeModel.Id);

                if (realCode == codeModel.Code)
                {
                    return Ok();
                }
                else { 
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("verifyaccount")]
        public IActionResult VerifyAccount(CodeModel account)
        {
            try
            {
                _userHandler.VerifyAccount(account.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("sendcollabmail")]
        public IActionResult SendCollabMail(MailModel userData)
        {
            try
            {
                string code = _codeHandler.CreateCode(userData.Id);
                string link = "http://localhost:8080/accept-invitation/";

                string title = "Invitación a colaborar en Business Tracker";
                string body = "<h1>Acepta tu invitación</h1>";
                body += "Hola!<br><br>Alguien piensa que puedes ser una parte clave de su emprendimiento.<br>";
                body += "Para aceptar la invitación accede al siguiente link ingresando este código:.<br><br>";
                body += "";
                body += "<b>" + code + "</b><br><br>";
                body += "";
                body += "<b>" + link + "</b><br><br>";
                body += "Si no has creado una cuenta en Business Tracker, puedes ignorar este correo.<br><br>";
                body += "Si tienes alguna pregunta, no dudes en contactarnos.<br><br>";
                body += "¡Bienvenido a bordo!<br><br>";
                body += "Saludos,<br>";
                body += "El equipo de Business Tracker";

                // Creando el correo
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(emailCollab),
                    Subject = title,
                    Body = body,
                };
                mailMessage.IsBodyHtml = true;

                // Seleccionando el destinatario
                mailMessage.To.Add(userData.Email);

                // Setteando la información necesaria para enviar el correo
                using var smtpClient = new SmtpClient();
                smtpClient.Host = host;
                smtpClient.Port = port;
                smtpClient.Credentials = new NetworkCredential(emailCollab, passwordCollab);
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
