﻿using Microsoft.AspNetCore.Mvc;
using BMT_backend.Handlers;
using BMT_backend.Infrastructure;
using BMT_backend.Domain.Entities;
using BMT_backend.Application.Services;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly OrderHandler _orderHandler;
        private readonly MailManager _mailManager;

        public UserController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _orderHandler = new OrderHandler(configuration);
            _mailManager = new MailManager(configuration);
        }
 
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest(new { Message = "El usuario no puede ser nulo." });

            try
            {
                var result = await _userService.CreateUserAsync(user);
                if (result)
                    return Ok(new { Success = true, Message = "Usuario creado exitosamente." });
                else
                    return StatusCode(500, new { Success = false, Message = "No se pudo crear el usuario." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(new { Success = true, Data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor." });
            }
        }

        [HttpGet("GetUserByUsername")]
        public async Task<IActionResult> GetUserByUsername([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(new { Message = "El nombre de usuario es obligatorio." });
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                if (user != null)
                    return Ok(new { Success = true, Data = user });
                else
                    return NotFound(new { Success = false, Message = "Usuario no encontrado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error al obtener el usuario." });
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDto updatedUser)
        {
            if (updatedUser == null)
                return BadRequest(new { Message = "Los datos del usuario actualizados no pueden ser nulos." });

            try
            {
                var result = await _userService.UpdateUserAsync(updatedUser);
                if (result)
                    return Ok(new { Success = true, Message = "Usuario actualizado exitosamente." });
                else
                    return StatusCode(500, new { Success = false, Message = "No se pudo actualizar el usuario." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("Role")]
        public async Task<IActionResult> UpdateRole([FromQuery] string id, [FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(role))
                return BadRequest(new { Message = "El ID del usuario y el rol son obligatorios." });

            try
            {
                var result = await _userService.UpdateRoleAsync(id, role);
                if (result)
                    return Ok(new { Success = true, Message = "Rol actualizado exitosamente." });
                else
                    return StatusCode(500, new { Success = false, Message = "No se pudo actualizar el rol del usuario." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error actualizando el rol del usuario." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginData)
        {
            if (loginData == null || string.IsNullOrWhiteSpace(loginData.Email) || string.IsNullOrWhiteSpace(loginData.Password))
                return BadRequest(new { Message = "El correo electrónico y la contraseña son obligatorios." });

            try
            {
                var (user, token) = await _userService.AuthenticateUserAsync(loginData.Email, loginData.Password);
                if (user != null && token != null)
                {
                    return Ok(new { Success = true, Token = token, User = user });
                }
                return Unauthorized(new { Success = false, Message = "Correo electrónico o contraseña inválidos." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor." });
            }
        }

        [HttpGet("GetToConfirmOrders")]
        public async Task<IActionResult> GetToConfirmOrders([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { Message = "El ID del usuario es obligatorio." });

            try
            {
                var toConfirmOrders = await Task.Run(() => _orderHandler.GetToConfirmUserOrders(userId));
                return Ok(new { Success = true, Data = toConfirmOrders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error al obtener las órdenes a confirmar." });
            }
        }

        [HttpPut("DenyOrder")]
        public async Task<IActionResult> DenyOrder([FromQuery] string orderID)
        {
            if (string.IsNullOrWhiteSpace(orderID))
                return BadRequest(new { Message = "El ID de la orden es obligatorio." });

            try
            {
                var denyResult = await Task.Run(() => _orderHandler.DenyOrder(orderID));
                if (denyResult)
                {
                    var order = await Task.Run(() => _orderHandler.GetOrderById(orderID));
                    if (order == null)
                        return NotFound(new { Success = false, Message = $"Orden con ID {orderID} no encontrada." });

                    await Task.Run(() => _mailManager.SendDenyEmail(order));
                    return Ok(new { Success = true, Message = "Orden cancelada y correo de notificación enviado." });
                }
                return BadRequest(new { Success = false, Message = "La cancelación de la orden falló." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Error al enviar el correo de notificación de cancelación." });
            }
        }
    }
}
