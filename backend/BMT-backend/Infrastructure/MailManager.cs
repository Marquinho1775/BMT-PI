using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Models;
using BMT_backend.Handlers;

namespace BMT_backend.Infrastructure
{
    public class MailManager
    {
        private readonly string email;
        private readonly string emailCollab;
        private readonly string host;
        private readonly int port;
        private readonly string password;
        private readonly string passwordCollab;
        public MailManager(IConfiguration configuration)
        {
            email = configuration["EmailSettings:Email"];
            emailCollab = configuration["EmailSettingsCollabRegister:Email"];
            host = configuration["EmailSettings:Host"];
            port = int.Parse(configuration["EmailSettings:Port"]);
            password = configuration["EmailSettings:Password"];
            passwordCollab = configuration["EmailSettingsCollabRegister:Password"];
        }

        public void SendConfirmationEmails(OrderModel order)
        {
            string title = "Confirmación de orden";
            string body = "<h1>Confirmación de orden</h1>";
            body += "Hola!<br><br>Gracias por tu orden en Business Tracker.<br>";
            body += "Tu orden ha sido confirmada.<br><br>";
            body += "Detalles de la orden:<br>";
            foreach (var product in order.Products)
            {
                body += "<b>Empresa:</b> " + product.EnterpriseName + "<br>";
                body += "<b>Producto:</b> " + product.ProductName + "<br>";
                body += "<b>Cantidad:</b> " + product.Quantity + "<br>";
                body += "<b>Fecha del producto:</b> " + product.ProductDate + "<br><br>";
            }
            body += "<b>Fecha de orden:</b> " + order.OrderDate + "<br>";
            body += "Si tienes alguna pregunta, no dudes en contactarnos.<br><br>";
            body += "¡Gracias por confiar en nosotros!<br><br>";
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
            mailMessage.To.Add(order.UserEmail);

            // Enviando el correo
            SmtpClient smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true,
            };
            smtpClient.Send(mailMessage);

            // Agrupar productos por empresa
            var productsByEnterprise = order.Products.GroupBy(p => new { p.EnterpriseName, p.EnterpriseEmail });

            // Enviar correo a cada empresa
            foreach (var group in productsByEnterprise)
            {
                string enterpriseBody = "<h1>Productos a preparar</h1>";
                enterpriseBody += "Hola!<br><br>Tienes nuevos productos para preparar.<br><br>";
                enterpriseBody += "Detalles de los productos:<br>";
                foreach (var product in group)
                {
                    enterpriseBody += "<b>Producto:</b> " + product.ProductName + "<br>";
                    enterpriseBody += "<b>Cantidad:</b> " + product.Quantity + "<br>";
                    enterpriseBody += "<b>Fecha del producto:</b> " + product.ProductDate + "<br><br>";
                }
                enterpriseBody += "Saludos,<br>";
                enterpriseBody += "El equipo de Business Tracker";

                // Creando el correo para la empresa
                MailMessage enterpriseMailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = "Productos a preparar",
                    Body = enterpriseBody,
                };
                enterpriseMailMessage.IsBodyHtml = true;
                enterpriseMailMessage.To.Add(group.Key.EnterpriseEmail);

                // Enviando el correo a la empresa
                smtpClient.Send(enterpriseMailMessage);
            }
        }
    }
}
