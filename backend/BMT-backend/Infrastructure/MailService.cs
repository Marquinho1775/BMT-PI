using System.Net.Mail;
using System.Net;
using BMT_backend.Domain.Entities;
using BMT_backend.Application.Interfaces;

namespace BMT_backend.Infrastructure
{
    public class MailService
    {
        private readonly string email;
        private readonly string emailCollab;
        private readonly string host;
        private readonly int port;
        private readonly string password;
        private readonly string passwordCollab;
        private readonly ICodeRepository _codeRepository;
            
        public MailService(IConfiguration configuration, ICodeRepository codeRepository)
        {
            email = configuration["EmailSettings:Email"];
            emailCollab = configuration["EmailSettingsCollabRegister:Email"];
            host = configuration["EmailSettings:Host"];
            port = int.Parse(configuration["EmailSettings:Port"]);
            password = configuration["EmailSettings:Password"];
            passwordCollab = configuration["EmailSettingsCollabRegister:Password"];
            _codeRepository = codeRepository;
        }

        public async Task SendVerificationEmail(Mail userData)
        {
            string code = await _codeRepository.CreateCodeAsync(userData.Id);
            string title = "Verificación de correo Business Tracker";
            string body = "<h1>Verifica tu correo</h1>" +
                          "Hola!<br><br>Gracias por registrarte en Business Tracker.<br>" +
                          "Para verificar tu cuenta, necesitamos que verifiques tu dirección de correo electrónico.<br><br>" +
                          $"<b>{code}</b><br><br>" +
                          "Si no has creado una cuenta en Business Tracker, puedes ignorar este correo.<br><br>" +
                          "Si tienes alguna pregunta, no dudes en contactarnos.<br><br>" +
                          "¡Bienvenido a bordo!<br><br>" +
                          "Saludos,<br>" +
                          "El equipo de Business Tracker";
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = title,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(userData.Email);

                using var smtpClient = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = true
                };
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar correo de verificación: {ex.Message}");
            }
        }
        //☆*: .｡. o(≧▽≦)o .｡.:*☆`(*>﹏<*)′o(*^▽^*)┛ヾ(＠⌒ー⌒＠)ノ＼(((￣(￣(￣▽￣)￣)￣)))／(((o(*ﾟ▽ﾟ*)o)))♪(´▽｀)o(*￣▽￣*)ブ（づ￣3￣）づ╭❤️～(

        public void SendCollabInviteEmail(CollabMail userData)
        {
            string link = "http://localhost:8080/acceptInvite/";
            string code = _codeRepository.CreateCodeAsync(userData.Id).ToString();
            string title = "Invitación a colaborar en Business Tracker";
            string body = "<h1>Acepta tu invitación</h1>" +
                          "Hola!<br><br>Alguien piensa que puedes ser una parte clave de su emprendimiento.<br>" +
                          "Para aceptar la invitación, accede al siguiente link ingresando estos códigos:<br><br>" +
                          $"<b>{userData.EntCode}</b><br><br>" +
                          $"<b>{code}</b><br><br>" +
                          $"<b>{link}</b><br><br>" +
                          "Si no has creado una cuenta en Business Tracker, o no está en tus planes colaborar con alguien, puedes ignorar este correo.<br><br>" +
                          "Si tienes alguna pregunta, no dudes en contactarnos.<br><br>" +
                          "¡Bienvenido a bordo!<br><br>" +
                          "Saludos,<br>" +
                          "El equipo de Business Tracker";
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailCollab),
                    Subject = title,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(userData.Email);

                using var smtpClient = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(emailCollab, passwordCollab),
                    EnableSsl = true
                };
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar correo de invitación de colaborador: {ex.Message}");
            }
        }

        public void SendConfirmedCollabEmail(Mail userData)
        {
            string title = "Un nuevo integrante a la familia de tu emprendimiento";
            string body = $"<h1>¡El usuario {userData.Id} ha aceptado colaborar contigo!</h1>" +
                          "¡Esperamos que juntos puedan cumplir sus sueños!<br><br>" +
                          "Saludos,<br>" +
                          "El equipo de Business Tracker";
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailCollab),
                    Subject = title,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(userData.Email);
                using var smtpClient = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(emailCollab, passwordCollab),
                    EnableSsl = true
                };
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar correo de confirmación de colaborador: {ex.Message}");
            }
        }

        public void SendConfirmationEmails(OrderDetails order)
        {
            SendClientEmail(order);
            SendEnterprisesEmails(order);
        }
        private void SendClientEmail(OrderDetails order)
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
            body += $"<b>Fecha de creación de orden:</b> {order.Order.OrderDate}<br>" +
                    $"<b>Fecha de Entrega:</b> {order.Order.DeliveryDate}<br>" +
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
                throw new Exception($"Error al enviar correo de confirmación al cliente: {ex.Message}");
            }
        }

        private void SendEnterprisesEmails(OrderDetails order)
        {
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
                try
                {
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
                catch (Exception ex)
                {
                    throw new Exception($"Error al enviar correo a la empresa: {ex.Message}");
                }
            }
        }

        public void SendDenyEmail(OrderDetails order)
        {
            string title = "Orden cancelada";
            string body = "<h1>Orden cancelada</h1>" +
                          $"Hola!<br><br>Te informamos que tu orden con la id {order.Order.OrderId} ha sido cancelada.<br>" +
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
