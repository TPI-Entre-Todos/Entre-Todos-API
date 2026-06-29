using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application;
using Application.Models;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _serviceNotificacion;

        // Inyectamos usando la interfaz perfectamente desacoplada
        public NotificacionController(INotificacionService serviceNotificacion)
        {
            _serviceNotificacion = serviceNotificacion;
        }

        // POST: api/Notificacion (Crear una notificación manual/sistema)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] NotificacionCreateDto dto)
        {
            var resultado = _serviceNotificacion.CrearNotificacion(dto);
            return Ok(resultado);
        }

        // GET: api/Notificacion (Traer las notificaciones del usuario autenticado)
        [HttpGet]
        public IActionResult GetAll()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var notificaciones = _serviceNotificacion.ObtenerPorUsuario(userId);
            return Ok(notificaciones);

        }

        // GET: api/Notificacion/usuario/3 (Traer las alertas de un usuario)
        [HttpGet("usuario/{usuarioId:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetPorUsuario(int usuarioId)
        {

            var notificaciones = _serviceNotificacion.ObtenerPorUsuario(usuarioId);
            return Ok(notificaciones);
        }

        // PUT: api/Notificacion/5/leer (Marcar como leída)
        [HttpPut("{id:int}/leer")]
        public IActionResult MarcarLeida(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            _serviceNotificacion.MarcarComoLeida(id, userId);
            return NoContent();

        }
    }
}
