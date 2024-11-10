using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using BMT_backend.Handlers;
using BMT_backend.Domain.Entities;

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

        public void SendConfirmationEmails(Order order)
        {
            string title = "Confirmación de orden";
            string body = "<h1>Confirmación de orden</h1>" +
                          "Hola!<br><br>Gracias por tu orden en Business Tracker.<br>" +
                          "Tu orden ha sido confirmada.<br><br>" +
                          "Detalles de la orden:<br>";

            foreach (var product in order.Products)
            {
                body += $"<b>Empresa:</b> {product.EnterpriseName}<br>" +
                        $"<b>Producto:</b> {product.ProductName}<br>" +
                        $"<b>Cantidad:</b> {product.Quantity}<br><br>";
            }

            body += $"<b>Fecha de creación de orden:</b> {order.OrderDate}<br>" +
                    $"<b>Fecha de Entrega:</b> {order.DeliveryDate}<br>" +
                    "Si tienes alguna pregunta, no dudes en contactarnos.<br><br>" +
                    "¡Gracias por confiar en nosotros!<br><br>" +
                    "Saludos,<br>" +
                    "El equipo de Business Tracker";

            try
            {
                using (var mailMessage = new MailMessage())
                using (var smtpClient = new SmtpClient(host, port))
                {
                    mailMessage.From = new MailAddress(email);
                    mailMessage.Subject = title;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(order.UserEmail);

                    smtpClient.Credentials = new NetworkCredential(email, password);
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mailMessage);
                }

                // Agrupar productos por empresa y enviar correos a cada empresa
                var productsByEnterprise = order.Products.GroupBy(p => new { p.EnterpriseName, p.EnterpriseEmail });

                foreach (var group in productsByEnterprise)
                {
                    string enterpriseBody = "<h1>Productos a preparar</h1>" +
                                            "Hola!<br><br>Tienes nuevos productos para preparar.<br><br>" +
                                            "Detalles de los productos:<br>";

                    foreach (var product in group)
                    {
                        enterpriseBody += $"<b>Producto:</b> {product.ProductName}<br>" +
                                          $"<b>Cantidad:</b> {product.Quantity}<br><br>";
                    }
                    enterpriseBody += "Saludos,<br>" +
                                      "El equipo de Business Tracker";

                    using (var enterpriseMailMessage = new MailMessage())
                    using (var smtpClient = new SmtpClient(host, port))
                    {
                        enterpriseMailMessage.From = new MailAddress(email);
                        enterpriseMailMessage.Subject = "Productos a preparar";
                        enterpriseMailMessage.Body = enterpriseBody;
                        enterpriseMailMessage.IsBodyHtml = true;
                        enterpriseMailMessage.To.Add(group.Key.EnterpriseEmail);

                        smtpClient.Credentials = new NetworkCredential(email, password);
                        smtpClient.EnableSsl = true;

                        smtpClient.Send(enterpriseMailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correos: {ex.Message}");
            }
        }

        public void SendDenyEmail(Order order)
        {
            string title = "Orden cancelada";
            string body = "<h1>Orden cancelada</h1>" +
                          $"Hola!<br><br>Te informamos que tu orden con la id {order.OrderId} ha sido cancelada.<br>" +
                          "Si tienes alguna pregunta, no dudes en contactarnos.<br><br>" +
                          "¡Gracias por confiar en nosotros!<br><br>" +
                          "Saludos,<br>" +
                          "El equipo de Business Tracker";
            try
            {
                using (var mailMessage = new MailMessage())
                using (var smtpClient = new SmtpClient(host, port))
                {
                    mailMessage.From = new MailAddress(email);
                    mailMessage.Subject = title;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(order.UserEmail);

                    smtpClient.Credentials = new NetworkCredential(email, password);
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar correo de cancelación: {ex.Message}");
            }
        }
    }
}
